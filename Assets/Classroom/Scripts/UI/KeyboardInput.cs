using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

using Microsoft.MixedReality.Toolkit.UI;

using Photon.Pun;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// This component links the NonNativeKeyboard to a TMP_InputField
    /// Put it on the TMP_InputField and assign the NonNativeKeyboard.prefab
    /// </summary>
    //[RequireComponent(typeof(TMP_InputField))]
    public class KeyboardInput : MonoBehaviour, IPointerDownHandler
    {
        [Experimental]
        [SerializeField] private NonNativeKeyboard keyboard = null;

        [SerializeField] private FollowMeToggle followMe = null;

        [SerializeField] private TextMeshProUGUI nameText = null;
        [SerializeField] private TextMeshProUGUI regionText = null;
        [SerializeField] private TextMeshProUGUI emailText = null;

        [SerializeField] private ColorPicker.ColorPicker colorPicker = null;
        [SerializeField] private GameObject colorPickerMesh;

        [SerializeField]
        private TextInputType textInput = TextInputType.name;

        // Store the PlayerPref Key to avoid typos
        const string userNamePrefKey = "UserName";
        const string userRegionPrefKey = "UserRegion";
        const string userEmailPrefKey = "UserEmail";
        const string userColorPrefKey = "UserColor";

        enum TextInputType
        {
            name,
            region,
            email
        }

        void Start()
        {
            string defaultName = string.Empty;
            string defaultRegion = string.Empty;
            string defaultEmail = string.Empty;
            Color defaultColor;

            defaultName = SetPlayerPrefsText(nameText, userNamePrefKey);
            defaultRegion = SetPlayerPrefsText(regionText, userRegionPrefKey);
            defaultEmail = SetPlayerPrefsText(emailText, userEmailPrefKey);
            defaultColor = SetPlayerPrefsColor();

            PhotonNetwork.NickName = defaultName;
        }

        public void SetUserMetaData()
        {
            ClassroomLauncher.userName = nameText.text;
            ClassroomLauncher.userRegion = regionText.text;
            ClassroomLauncher.userEmail = emailText.text;

            if (colorPicker.colorAssigned)
            {
                ClassroomLauncher.userColor = colorPicker.CustomColor;
                SaveColor(ClassroomLauncher.userColor, userColorPrefKey);
            }
        }

        public void InputName()
        {
            textInput = TextInputType.name;
            OpenKeyboard();
        }

        public void InputRegion()
        {
            textInput = TextInputType.region;
            OpenKeyboard();
        }

        public void InputEmail()
        {
            textInput = TextInputType.email;
            OpenKeyboard();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OpenKeyboard();
        }

        private string SetPlayerPrefsText(TextMeshProUGUI textMesh, string prefKey)
        {
            string defaultText = string.Empty;

            if (textMesh != null)
            {
                if (PlayerPrefs.HasKey(prefKey))
                {
                    defaultText = PlayerPrefs.GetString(prefKey);
                    textMesh.text = defaultText;
                }
            }

            return (defaultText);
        }

        private Color SetPlayerPrefsColor()
        {
            Color defaultColor = Color.red;

            if (colorPicker.CustomColor != null)
            {
                if (PlayerPrefs.HasKey(userColorPrefKey))
                {
                    defaultColor = GetSaveColor(userColorPrefKey);
                    colorPicker.CustomColor = defaultColor;
                    colorPicker.SetTargetColors(colorPickerMesh);
                    colorPickerMesh.SetActive(false);
                    colorPicker.colorAssigned = true;
                }
            }

            return (defaultColor);
        }

        public static Color GetSaveColor(string key)
        {
            string col = PlayerPrefs.GetString(key);
            Debug.Log(col);
            if (col == "")
            {
                return Color.black;
            }
            string[] strings = col.Split(',');
            Color output = new Color();
            for (int i = 0; i < 4; i++)
            {
                output[i] = System.Single.Parse(strings[i]);
            }
            return output;
        }

        public static void SaveColor(Color color, string key)
        {
            string col = color.ToString();
            col = col.Replace("RGBA(", "");
            col = col.Replace(")", "");
            PlayerPrefs.SetString(key, col);
        }

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        private void SetUserName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }

            PlayerPrefs.SetString(userNamePrefKey, value);
            PhotonNetwork.NickName = value;
        }
        private void SetUserRegion(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }

            PlayerPrefs.SetString(userRegionPrefKey, value);
            //PhotonNetwork.NickName = value;
        }
        private void SetUserEmail(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }

            PlayerPrefs.SetString(userEmailPrefKey, value);
            //PhotonNetwork.NickName = value;
        }

        private void OpenKeyboard()
        {
            keyboard.PresentKeyboard();

            keyboard.OnClosed += DisableKeyboard;
            keyboard.OnTextSubmitted += DisableKeyboard;
            keyboard.OnTextUpdated += UpdateText;

            followMe.SetFollowMeBehavior(false);
        }

        private void UpdateText(string text)
        {
            UpdateTextByInputType(text);
            //GetComponent<TMP_InputField>().text = text;
        }

        private void UpdateTextByInputType(string text)
        {
            switch(textInput)
            {
                case TextInputType.name:
                    if(nameText == null)
                    {
                        Debug.LogError("MISSING NAMETEXT");
                        return;
                    }
                    nameText.text = text;
                    SetUserName(text);
                    break;
                case TextInputType.region:
                    if (regionText == null)
                    {
                        Debug.LogError("MISSING REGIONTEXT");
                        return;
                    }
                    regionText.text = text;
                    SetUserRegion(text);
                    break;
                case TextInputType.email:
                    if (emailText == null)
                    {
                        Debug.LogError("MISSING EMAILTEXT");
                        return;
                    }
                    emailText.text = text;
                    SetUserEmail(text);
                    break;
            }
        }

        private void DisableKeyboard(object sender, EventArgs e)
        {
            keyboard.OnTextUpdated -= UpdateText;
            keyboard.OnClosed -= DisableKeyboard;
            keyboard.OnTextSubmitted -= DisableKeyboard;

            followMe.SetFollowMeBehavior(true);

            keyboard.Close();
        }
    }
}