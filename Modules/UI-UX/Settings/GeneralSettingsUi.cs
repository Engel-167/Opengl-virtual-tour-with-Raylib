using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Settings;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Settings;

public class GeneralSettingsUi
{
    private readonly Container? _container;

    public GeneralSettingsUi()
    {
        Button languageButton = new Button(
                Textures.EnabledStateButton,
                Textures.EnabledStateButton,
                new Vector2(0, 0), 200, 80,
                [15, 15, 15, 15])
        {
            Text = Variables.Language.LanguageString,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        languageButton.Event += (_, _) =>
        {
            string currentLanguage = Variables.AppSettings.Language;
            if (currentLanguage == "EN")
            {
                currentLanguage = "ES";
            }
            else
            {
                currentLanguage = "EN";
            }

            Console.WriteLine($"Current language: {currentLanguage}");
            
            Variables.AppSettings.Language = currentLanguage;
            SettingsLoader.SaveSettings(Variables.SettingsFilePath, Variables.AppSettings);

            if (Variables.AppSettings.Language == "EN")
            {
                Variables.Language = SettingsLoader.LoadLanguage(Variables.EnglishLangFilePath);
            }
            
            if(Variables.AppSettings.Language == "ES")
            {
                Variables.Language = SettingsLoader.LoadLanguage(Variables.SpanishLangFilePath);
            }

            Variables.UpdateText = true;
            
            Variables.HomeUi?.UpdateText();
        };

        _container = new Container(ContainerOrientation.Horizontal, 10, 5);
        _container.Components.Add(languageButton);
        
        _container.Position = new Vector2(
            Raylib.GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f,
            Raylib.GetScreenHeight() / 2.0f - _container.GetHeight() / 2.0f
        );
    }

    public void Draw()
    {
        UpdateLayout();
        if (Variables.UpdateText)
        {
            if (_container != null)
            {
                string text = Variables.Language.LanguageString;
                
                _container.Components[0].Text = text;
            }
        }
        
        _container?.Draw();
    }
    
    public void UpdateLayout()
    {
        if (_container == null) return;

        _container.Position = new Vector2(
            Raylib.GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f,
            Raylib.GetScreenHeight() / 2.0f - _container.GetHeight() / 2.0f
        );
    }
}