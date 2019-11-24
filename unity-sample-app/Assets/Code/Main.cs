using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour, SingularLinkHandler {
    void Start() {

    }

    void Update() {
    }

    public void btnInitSDK_OnClick() {
        // If you want to initialize the SDK manually, don't forget to turn off the InitializeOnAwake flag on the SDK object
        SingularSDK.InitializeSingularSDK();
    }

    public void btnSendEvent_OnClick() {
        // Reporting a simple event to Singular
        SingularSDK.Event("Test Event");
    }

    public void btnSendEventWithArgs_OnClick() {
        var attributes = new Dictionary<string, object>() {
            ["key1"] = "value1",
            ["key2"] = "value2"
        };

        // Reporting a simple event with your custom attributes and values to pass with the event
        SingularSDK.Event(attributes, "Test Event With Args");
    }

    public void btnSendRevenue_OnClick() {
        // Reporting a simple revenue event to Singular of $4.99
        SingularSDK.CustomRevenue("Test Revenue", "USD", 4.99);
    }

    public void btnSendInAppPurchase_OnClick() {
        var attributes = new Dictionary<string, object>() {
            ["key1"] = "value1",
            ["key2"] = "value2"
        };

        //SingularSDK.InAppPurchase("Test IAP", null, attributes);
    }

    public void btnSetCustomUserId_OnClick() {
        // Once set, the Custom User Id will persist until `SingularSDK.UnsetCustomUserId()` will be called.
        // This can also be called before SDK init if you want the first session to include the Custom User Id.
        SingularSDK.SetCustomUserId("My Custom User Id");
    }

    // Implmenting this method will support Deferred Deep Links out of the box.
    // If you want to fully support Singular links please follow the instructions here:https://developers.singular.net/docs/unity-sdk#section-singular-links.
    public void OnSingularLinkResolved(SingularLinkParams linkParams) {

        // The deeplink value that was set on the link
        string deeplink = linkParams.Deeplink;

        // Passthrough param that was set on the link
        string passthrough = linkParams.Passthrough;

        // A flag that indicates wether the deeplink was deferred or not
        bool isLinkDeferred = linkParams.IsDeferred;
    }
}
