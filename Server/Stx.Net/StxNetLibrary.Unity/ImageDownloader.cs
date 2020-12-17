using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Stx.Net.Unity
{
    public class ImageDownloader
    {
        /*public static IEnumerator DownloadTo(string url, Image ui)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;

                try
                {
                    Texture2D tex = www.texture;
                    ui.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                }
                catch
                { }
            }
        }*/

        public static IEnumerator DownloadTo(string url, Image to)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                if (texture == null)
                    to.sprite = null;
                else
                    to.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }
    }
}
