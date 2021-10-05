using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;
using System.Text.RegularExpressions;
public class LocaleManager
{

    private bool inited = false;
    private string langCode = "EN";
    private string defaultLang = "EN";

    private Dictionary<string, object> library;

    public string OVERRIDE_LOCALE = "";
    
    public void init(string jsonString)
    {
        if (inited)
            return;

        OVERRIDE_LOCALE = "";

        langCode = LanguageHelper.Get2LetterISOCodeFromSystemLanguage();

        if(Application.isEditor)
        {
            langCode = defaultLang;
        }

        this.library = Json.Deserialize(jsonString) as Dictionary<string, object>;
        inited = true;


        Debug.Log("[LocaleManager] Inited with language: " + langCode);

    }

    public void setLanguage(string langCode)
    {
        this.langCode = langCode;

    }

    public string fixText(string input,bool toUpper = false)
    {

        ///Regex.Match("User name (sales)", @"\{([^)]*)\}").Groups[1].Value

        string output = input;
        foreach (Capture c in Regex.Match(input, @"\{([^)]*)\}").Groups)
        {
            output = output.Replace("{" + c.Value + "}", getText(c.Value));
        }


        if(toUpper)
        {
            output = output.ToUpper();
        }
        // Format is "{id}" example "{buy }" + 200 + {coins}"; ''

        return output;
    }

    public string getText(string id, bool toUpper = false)
    {
        string output = "";

        string lc = langCode;

        if(OVERRIDE_LOCALE != "")
        {
            lc = OVERRIDE_LOCALE;
            Debug.LogWarning("[LOCALIZATION] USING OVERRIDE LOCALE:" + OVERRIDE_LOCALE);
        }

        if(library.ContainsKey(id))
        {
            Dictionary<string, object> word = library[id] as Dictionary<string,object>;
            if(word.ContainsKey(lc))
            {
                output = word[lc] as string;
            }

            if(output == "")
            {
                if (word.ContainsKey(defaultLang))
                {
                    output = word[defaultLang] as string;
                } else if(word.ContainsKey("EN"))
                {
                    output = word["EN"] as string;
                }
                else
                {
                    Debug.LogWarning("[Localization] id not found: " + id);
                }
            }

        }

        if(toUpper)
        {
            output = output.ToUpper();
        }

        return output;
    }

}
