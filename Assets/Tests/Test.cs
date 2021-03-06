﻿using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using UnityAndroidNative.Android.App;
using UnityAndroidNative.Android.Content;
using UnityAndroidNative.Android.Content.Pm;
using UnityAndroidNative.Android.Os;
using UnityAndroidNative.Android.Widget;
using UnityAndroidNative.Private;
#endif
using UnityEngine;

public class Test : MonoBehaviour {

    private void Start() {
        //        Share("test", "test title");
        ShowDialog("Hello there", "click me");
    }

    private void Update() {

    }

    public static void ShowDialog(string title, string msg) {
#if UNITY_ANDROID
        var activity = Internal.GetCurrentActivity();
        AlertDialog alertDialog = new AlertDialog.Builder(activity).Create();
        alertDialog.SetTitle(title);
        alertDialog.SetMessage(msg);
        alertDialog.SetButton(Dialog.BUTTON_NEGATIVE, "No", new DialogOnClickListener(which => {
            Debug.LogWarning("btn clicked: " + which);
        }));
        alertDialog.SetButton(Dialog.BUTTON_POSITIVE, "Yes", new DialogOnClickListener(which => {
            Debug.LogWarning("btn clicked: " + which);
        }));
        alertDialog.Show();
#endif
    }

    public static void Share(string body, string subject, string mimeType = "text/plain", string chooserTitle = "Choose application") {
#if UNITY_ANDROID
        //        Debug.LogWarning(Build.VERSION.SDK_INT + " => " + Build.VERSION.CODENAME);
        Intent intent = new Intent(Intent.ACTION_SEND);
        intent.SetType(mimeType)
            .PutExtra(Intent.EXTRA_SUBJECT, subject)
            .PutExtra(Intent.EXTRA_TEXT, body);

        var intentList = new List<Intent>();

        var activity = Internal.GetCurrentActivity();
        var pm = activity.GetPackageManager();
        var resInfo = pm.QueryIntentActivities(intent, 0);


        for (int i = 0; i < resInfo.Count; i++) {
            // Extract the label, append it, and repackage it in a LabeledIntent
            ResolveInfo ri = resInfo[i];

            string packageName = ri.ActivityInfo.PackageName;
            if (packageName.Contains("vkontakte")  || packageName.Contains("instagram") || packageName.Contains("skype")) {
                Intent newIntent = new Intent(Intent.ACTION_SEND);
                newIntent.SetComponent(new ComponentName(packageName, ri.ActivityInfo.Name));
                newIntent.SetPackage(packageName)
                    .PutExtra(Intent.EXTRA_SUBJECT, subject)
                    .PutExtra(Intent.EXTRA_TEXT, body)
                    .SetType(mimeType);

                intentList.Add(new LabeledIntent(newIntent, packageName, ri.LoadLabel(pm) + " [" + i + "]", ri.Icon));
            }
        }

        Intent intentt = intentList[0];
        intentList.RemoveAt(0);
        Parcelable[] extraIntents = new Parcelable[intentList.Count];
       
        for (int i = 0; i < intentList.Count; i++) {
            extraIntents[i] = intentList[i];
        }

        var chooser = Intent.CreateChooser(intentt, chooserTitle);
        chooser.PutExtra(Intent.EXTRA_INITIAL_INTENTS, extraIntents);
        activity.StartActivity(chooser);
#endif
    }
}
