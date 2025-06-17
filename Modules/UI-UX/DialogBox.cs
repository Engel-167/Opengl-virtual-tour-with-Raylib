using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public class DialogBox
{
    private readonly Panel _backgroundPanel;
    private readonly Button _closeButton;

    public DialogBox()
    {
        _backgroundPanel = new Panel(
            Textures.BasicPanel,
            new Vector2(20, Raylib.GetScreenHeight()/2f),  // Position
            Raylib.GetScreenWidth() - 40, Raylib.GetScreenHeight() /2 - 20, // Width and Height
            [40, 40, 40, 40] // Padding
        )
        {
            Text = "",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        
        _closeButton = new Button(
            Textures.BlueExitButton,
            Textures.BlueExitButtonFocus,
            new Vector2(_backgroundPanel.Width - 30, Raylib.GetScreenHeight()/2.0f - 10), 60, 60,
            [0, 0, 0, 0])
        {
            Text = string.Empty,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        _closeButton.Event += (_, _) =>
        {
            Variables.IsDialogBoxEnabled = false;
            Console.WriteLine("Close button DialogBox");
        };
    }
    
    public void Draw()
    {
        UpdateLayout();
        _backgroundPanel.Draw();
        
        // In DialogBox.cs, inside the Draw() method, after _backgroundPanel.Draw():
        var textRect = new Rectangle(
            _backgroundPanel.Position.X + 40,
            _backgroundPanel.Position.Y + 40,
            _backgroundPanel.Width - 60,
            _backgroundPanel.Height - 60
        );
        
        DrawTextBoxed(
            _backgroundPanel.Font,
            Variables.Dialogs.Dialog1,
            textRect,
            _backgroundPanel.FontSize,
            _backgroundPanel.FontSpacing,
            true, // wordWrap
            Color.Black
        );
        
        _closeButton.Draw();
    }

    private void UpdateText()
    {
        _backgroundPanel.Text = "";
    }

    public void UpdateLayout()
    {
        UpdateText();
        _backgroundPanel.Position = new Vector2(20, Raylib.GetScreenHeight()/2f);
        _backgroundPanel.Width = Raylib.GetScreenWidth() - 40;
        _backgroundPanel.Height = Raylib.GetScreenHeight() / 2f - 20;
        
        _closeButton.Position = new Vector2(_backgroundPanel.Width - 30, Raylib.GetScreenHeight() / 2.0f - 10);
    }
    
    static void DrawTextBoxed(
        Font font,
        string text,
        Rectangle rec,
        float fontSize,
        float spacing,
        bool wordWrap,
        Color tint
    )
    {
        DrawTextBoxedSelectable(font, text, rec, fontSize, spacing, wordWrap, tint, 0, 0, Color.White, Color.White);
    }

    // Draw text using font inside rectangle limits with support for text selection
    static unsafe void DrawTextBoxedSelectable(
        Font font,
        string text,
        Rectangle rec,
        float fontSize,
        float spacing,
        bool wordWrap,
        Color tint,
        int selectStart,
        int selectLength,
        Color selectTint,
        Color selectBackTint
    )
    {
        int length = text.Length;

        // Offset between lines (on line break '\n')
        float textOffsetY = 0;

        // Offset X to the next character to draw
        float textOffsetX = 0.0f;

        // Character rectangle scaling factor
        float scaleFactor = fontSize / font.BaseSize;

        // Word/character wrapping mechanism variables
        bool shouldMeasure = wordWrap;

        // Index where to begin drawing (where a line begins)
        int startLine = -1;

        // Index where to stop drawing (where a line ends)
        int endLine = -1;

        // Holds last value of the character position
        int lastk = -1;

        using var textNative = new Utf8Buffer(text);

        for (int i = 0, k = 0; i < length; i++, k++)
        {
            // Get the next codepoint from byte string and glyph index in font
            int codepointByteCount = 0;
            int codepoint = Raylib.GetCodepoint(&textNative.AsPointer()[i], &codepointByteCount);
            int index = Raylib.GetGlyphIndex(font, codepoint);

            // NOTE: Normally we exit the decoding sequence as soon as a bad byte is found (and return 0x3f)
            // but we need to draw all the bad bytes using the '?' symbol moving one byte
            if (codepoint == 0x3f)
            {
                codepointByteCount = 1;
            }

            i += (codepointByteCount - 1);

            float glyphWidth = 0;
            if (codepoint != '\n')
            {
                glyphWidth = (font.Glyphs[index].AdvanceX == 0) ?
                    font.Recs[index].Width * scaleFactor :
                    font.Glyphs[index].AdvanceX * scaleFactor;

                if (i + 1 < length)
                {
                    glyphWidth = glyphWidth + spacing;
                }
            }

            // NOTE: When wordWrap is ON we first measure how much of the text we can draw before going outside 
            // the rec container. We store this info in startLine and endLine, then we change states, draw the text
            // between those two variables and change states again and again recursively until the end of the text
            // (or until we get outside of the container). When wordWrap is OFF we don't need the measure state so
            // we go to the drawing state immediately and begin drawing on the next line before we can get outside
            // the container.
            if (shouldMeasure)
            {
                if ((codepoint == ' ') || (codepoint == '\t') || (codepoint == '\n'))
                {
                    endLine = i;
                }

                if ((textOffsetX + glyphWidth) > rec.Width)
                {
                    endLine = (endLine < 1) ? i : endLine;
                    if (i == endLine)
                    {
                        endLine -= codepointByteCount;
                    }
                    if ((startLine + codepointByteCount) == endLine)
                    {
                        endLine = (i - codepointByteCount);
                    }

                    shouldMeasure = !shouldMeasure;
                }
                else if ((i + 1) == length)
                {
                    endLine = i;
                    shouldMeasure = !shouldMeasure;
                }
                else if (codepoint == '\n')
                {
                    shouldMeasure = !shouldMeasure;
                }

                if (!shouldMeasure)
                {
                    textOffsetX = 0;
                    i = startLine;
                    glyphWidth = 0;

                    // Save character position when we switch states
                    int tmp = lastk;
                    lastk = k - 1;
                    k = tmp;
                }
            }
            else
            {
                if (codepoint == '\n')
                {
                    if (!wordWrap)
                    {
                        textOffsetY += (font.BaseSize + font.BaseSize / 2f) * scaleFactor;
                        textOffsetX = 0;
                    }
                }
                else
                {
                    if (!wordWrap && ((textOffsetX + glyphWidth) > rec.Width))
                    {
                        textOffsetY += (font.BaseSize + font.BaseSize / 2f) * scaleFactor;
                        textOffsetX = 0;
                    }

                    // When text overflows rectangle height limit, just stop drawing
                    if ((textOffsetY + font.BaseSize * scaleFactor) > rec.Height)
                    {
                        break;
                    }

                    // Draw selection background
                    bool isGlyphSelected = false;
                    if ((selectStart >= 0) && (k >= selectStart) && (k < (selectStart + selectLength)))
                    {
                        Raylib.DrawRectangleRec(
                            new Rectangle(
                                rec.X + textOffsetX - 1,
                                rec.Y + textOffsetY,
                                glyphWidth,
                                font.BaseSize * scaleFactor
                            ),
                            selectBackTint
                        );
                        isGlyphSelected = true;
                    }

                    // Draw current character glyph
                    if ((codepoint != ' ') && (codepoint != '\t'))
                    {
                        Raylib.DrawTextCodepoint(
                            font,
                            codepoint,
                            new Vector2(rec.X + textOffsetX, rec.Y + textOffsetY),
                            fontSize,
                            isGlyphSelected ? selectTint : tint
                        );
                    }
                }

                if (wordWrap && i == endLine)
                {
                    textOffsetY += (font.BaseSize + font.BaseSize / 2f) * scaleFactor;
                    textOffsetX = 0;
                    startLine = endLine;
                    endLine = -1;
                    glyphWidth = 0;
                    selectStart += lastk - k;
                    k = lastk;

                    shouldMeasure = !shouldMeasure;
                }
            }

            if ((textOffsetX != 0) || (codepoint != ' '))
            {
                // avoid leading spaces
                textOffsetX += glyphWidth;
            }
        }
    }
}