using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.IO;

class MonoBehaviourGameObject : MonoBehaviour {

}

public class LocalizationImportTool : EditorWindow
{
    string localFilePath = @"/VacuumMaster/locale.json";
    string dataSheetId = "1T8gGY3JLhNGUtuGwCuM5sHLG7DLt5I1EyYT62bE9Dcw";
    string IdsToReplace = "ZH-TW,ZT;";
    string csvToJsonConverterURL = "https://asg-unity.herokuapp.com";
    string textLog;
    bool isRunning = false;

    private MonoBehaviourGameObject go;

    [MenuItem("Tools/Localization Import Tool")]
    public static void ShowWindow() {
        GetWindow<LocalizationImportTool>("Localization Import Tool");
    }


    void OnGUI() {
        localFilePath = EditorGUILayout.TextField("Localization file path", localFilePath);
        dataSheetId = EditorGUILayout.TextField("Data Sheet Id", dataSheetId);
        IdsToReplace = EditorGUILayout.TextField("IDs to replace", IdsToReplace);
        csvToJsonConverterURL = EditorGUILayout.TextField("CSV to JSON converter url", csvToJsonConverterURL);

        var buttonStyle = new GUIStyle(GUI.skin.button);
        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(isRunning)) {
            if (GUILayout.Button("Import", buttonStyle, GUILayout.MaxWidth(200))) {

                textLog = "";
                if (Application.isPlaying) {
                    textLog = "Can't run script: Application is running";
                } else {
                    go = new GameObject().AddComponent<MonoBehaviourGameObject>();
                    go.gameObject.name = "WebRequest";
                    go.StartCoroutine(DoFileRequest());
                }
            }
        }

        EditorGUILayout.Space();

        var labelFieldStyle = new GUIStyle(GUI.skin.label) {
            alignment = TextAnchor.UpperLeft,
            normal = {
                background = Texture2D.whiteTexture,
                textColor = Color.black
            }
        };

        EditorGUILayout.SelectableLabel(textLog, labelFieldStyle, GUILayout.ExpandHeight(true));
    }

    private IEnumerator DoFileRequest() {
        isRunning = true;
        textLog = "Fetching localization data...";
        string url = $"{csvToJsonConverterURL}/?docId={dataSheetId}&replace={IdsToReplace}";
        UnityWebRequest webRequest = new UnityWebRequest(url);

        byte[] bytes = System.Text.Encoding.ASCII.GetBytes("");

        DownloadHandlerBuffer dH = new DownloadHandlerBuffer();

        webRequest.downloadHandler = dH;
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.Send();


        if (webRequest.result != UnityWebRequest.Result.Success) {
            textLog += "\nData not found";
            textLog += "\n" + webRequest.error;
            isRunning = false;
        } else {
            textLog += "\nData received.";
            SaveFileData(webRequest.downloadHandler.text);
            isRunning = false;
        }
        DestroyImmediate(go.gameObject);
    }

    private void SaveFileData(string data) {
        textLog += "\nSaving data...";

        if (System.IO.File.Exists(Application.dataPath + localFilePath) == false) {
            textLog += "\nFile could not be found.";
        } else {
            using (var stream = new FileStream(Application.dataPath + localFilePath, FileMode.Truncate)) {
                using (var writer = new StreamWriter(stream)) {
                    writer.Write(data);
                    textLog += "\n\nDone!";
                }
            }
        }

    }
}
