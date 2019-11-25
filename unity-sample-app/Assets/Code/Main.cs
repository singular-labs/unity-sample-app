using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class Main : MonoBehaviour, SingularLinkHandler {

    public Text _toast;
    private Coroutine coroutine;

    void Start() {
        SetToastOpacity(0);
    }

    void Update() {
    }

    #region SDK Initialization

    public void InitSDK_OnClick() {
        // Here we initialize the SDK manually, instead using the InitializeOnAwake flag on the SDK Object.
        // By default InitializeOnAwake is true, which will automatically initialize the SDK when the scene loads.
        SingularSDK.InitializeSingularSDK();

        ShowToast("SDK initialized");
    }

    #endregion

    #region Custom User Id

    public void SetCustomUserId_OnClick() {
        string customUserId = "johndoe@gmail.com";
        // Once set, the Custom User Id will persist between runs until `SingularSDK.UnsetCustomUserId()` is called.
        // This can also be called before SDK init if you want the first session to include the Custom User Id.
        SingularSDK.SetCustomUserId(customUserId);

        ShowToast($"Custom User Id was set to '{customUserId}'");
    }

    #endregion

    #region Custom Events

    public void SendEvent_OnClick() {
        // Reporting a simple event to Singular
        SingularSDK.Event("Test Event");

        ShowToast("Event sent");
    }

    public void SendEventWithArgs_OnClick() {
        var attributes = new Dictionary<string, object>() {
            ["key1"] = "value1",
            ["key2"] = "value2"
        };

        // Reporting a simple event with your custom attributes to pass with the event
        SingularSDK.Event(attributes, "Test Event With Args");

        ShowToast("Event with args sent");
    }

    #region Revenue Events

    public void SendRevenue_OnClick() {
        // Reporting a simple revenue event to Singular of $4.99
        SingularSDK.CustomRevenue("Test Revenue", "USD", 4.99);

        ShowToast("Revenue event sent");
    }

    #endregion

    public void SendInAppPurchase_OnClick() {
        var attributes = new Dictionary<string, object>() {
            ["key1"] = "value1",
            ["key2"] = "value2"
        };

        // Instead of sending a real product we create a fake one for testing.
        // In your production environment, the Product object should be received from the Unity Purchasing API. 
        Product product = BuildProduct();
        SingularSDK.InAppPurchase("Test IAP", product, attributes);

        ShowToast("IAP sent");
    }

    #endregion

    #region Singular Links

    // This is the handler for Singular Links deep links.
    // Without implementing any additional native support, only deferred deep links will be resolved here. 
    // To see how to fully support Singular Links please read here: https://developers.singular.net/docs/unity-sdk#section-singular-links.
    public void OnSingularLinkResolved(SingularLinkParams linkParams) {

        // The deeplink value that was set on the link
        string deeplink = linkParams.Deeplink;

        // Passthrough param that was set on the link
        string passthrough = linkParams.Passthrough;

        // A flag that indicates wether the deeplink was deferred or not
        bool isLinkDeferred = linkParams.IsDeferred;
    }

    #endregion

    #region Build Fake Product

    private static Product BuildProduct() {
        PayoutDefinition payout = new PayoutDefinition("subtype", 3);
        ProductDefinition definition = new ProductDefinition("product_id", "store_id", ProductType.Consumable, true, payout);
        ProductMetadata metadata = new ProductMetadata("15.00", "my_product", "prodcut decription", "USD", decimal.Parse("15.00"));

        var ctor = typeof(Product).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ProductDefinition), typeof(ProductMetadata), typeof(string) }, null);
        Product product = ctor.Invoke(new object[] { definition, metadata, IOSProductReceipt }) as Product;

        return product;
    }

    private static string IOSProductReceipt = "{\"Store\":\"AppleAppStore\",\"TransactionID\":\"1000000577718845\",\"Payload\":\"MIIVRAYJKoZIhvcNAQcCoIIVNTCCFTECAQExCzAJBgUrDgMCGgUAMIIE5QYJKoZIhvcNAQcBoIIE1gSCBNIxggTOMAoCAQgCAQEEAhYAMAoCARQCAQEEAgwAMAsCAQECAQEEAwIBADALAgELAgEBBAMCAQAwCwIBDgIBAQQDAgFvMAsCAQ8CAQEEAwIBADALAgEQAgEBBAMCAQAwCwIBGQIBAQQDAgEDMAwCAQMCAQEEBAwCNjEwDAIBCgIBAQQEFgI0KzANAgENAgEBBAUCAwHVKDANAgETAgEBBAUMAzEuMDAOAgEJAgEBBAYCBFAyNTIwGAIBBAIBAgQQsnvWKWlDGMM9JNi9HV9UBzAbAgEAAgEBBBMMEVByb2R1Y3Rpb25TYW5kYm94MBwCAQUCAQEEFCj0pxpeDli0m0tbcFIlIhSc0wbRMB4CAQwCAQEEFhYUMjAxOS0wNi0xN1QwNzo0MjoyM1owHgIBEgIBAQQWFhQyMDEzLTA4LTAxVDA3OjAwOjAwWjAjAgECAgEBBBsMGWNvbS5wbGF5c3RhY2sucmVzY3Vld2luZ3MwQAIBBwIBAQQ4ThLbjN4MBagzCX6iocF77GKaXoLpGt+qXG1GPcI8510UbqksxS6w+OTcXZusLDjR+v5r3lNChZgwTAIBBgIBAQREZttt6TsyOE1daN54cyKrPhcUk+vAxf50E4rCFHf65Ag2OI9oOJhrzMG3hVLtOAS4eaTib2j5woofLVwYcPAKFCyLUHEwggFRAgERAgEBBIIBRzGCAUMwCwICBqwCAQEEAhYAMAsCAgatAgEBBAIMADALAgIGsAIBAQQCFgAwCwICBrICAQEEAgwAMAsCAgazAgEBBAIMADALAgIGtAIBAQQCDAAwCwICBrUCAQEEAgwAMAsCAga2AgEBBAIMADAMAgIGpQIBAQQDAgEBMAwCAgarAgEBBAMCAQAwDAICBq4CAQEEAwIBADAMAgIGrwIBAQQDAgEAMAwCAgaxAgEBBAMCAQAwFwICBqYCAQEEDgwMc3RhcnRlcl9wYWNrMBsCAganAgEBBBIMEDEwMDAwMDA1Mzc3MDc4NjgwGwICBqkCAQEEEgwQMTAwMDAwMDUzNzcwNzg2ODAfAgIGqAIBAQQWFhQyMDE5LTA2LTE3VDA3OjMzOjQ1WjAfAgIGqgIBAQQWFhQyMDE5LTA2LTE3VDA3OjMzOjQ1WjCCAXsCARECAQEEggFxMYIBbTALAgIGrQIBAQQCDAAwCwICBrACAQEEAhYAMAsCAgayAgEBBAIMADALAgIGswIBAQQCDAAwCwICBrQCAQEEAgwAMAsCAga1AgEBBAIMADALAgIGtgIBAQQCDAAwDAICBqUCAQEEAwIBATAMAgIGqwIBAQQDAgEDMAwCAgauAgEBBAMCAQAwDAICBrECAQEEAwIBADAMAgIGtwIBAQQDAgEAMBICAgavAgEBBAkCBwONfqd1ol4wGQICBqYCAQEEEAwOc3Vic2NyaXB0aW9uXzIwGwICBqcCAQEEEgwQMTAwMDAwMDUzNzcxMTg0NTAbAgIGqQIBAQQSDBAxMDAwMDAwNTM3NzExODQ1MB8CAgaoAgEBBBYWFDIwMTktMDYtMTdUMDc6NDI6MjJaMB8CAgaqAgEBBBYWFDIwMTktMDYtMTdUMDc6NDI6MjNaMB8CAgasAgEBBBYWFDIwMTktMDYtMTdUMDc6NDU6MjJaoIIOZTCCBXwwggRkoAMCAQICCA7rV4fnngmNMA0GCSqGSIb3DQEBBQUAMIGWMQswCQYDVQQGEwJVUzETMBEGA1UECgwKQXBwbGUgSW5jLjEsMCoGA1UECwwjQXBwbGUgV29ybGR3aWRlIERldmVsb3BlciBSZWxhdGlvbnMxRDBCBgNVBAMMO0FwcGxlIFdvcmxkd2lkZSBEZXZlbG9wZXIgUmVsYXRpb25zIENlcnRpZmljYXRpb24gQXV0aG9yaXR5MB4XDTE1MTExMzAyMTUwOVoXDTIzMDIwNzIxNDg0N1owgYkxNzA1BgNVASDMLk1hYyBBcHAgU3RvcmUgYW5kIGlUdW5lcyBTdG9yZSBSZWNlaXB0IFNpZ25pbmcxLDAqBgNVBAsMI0FwcGxlIFdvcmxkd2lkZSBEZXZlbG9wZXIgUmVsYXRpb25zMRMwEQYDVQQKDApBcHBsZSBJbmMuMQswCQYDVQQGEwJVUzCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKXPgf0looFb1oftI9ozHI7iI8ClxCbLPcaf7EoNVYb/pALXl8o5VG19f7JUGJ3ELFJxjmR7gs6JuknWCOW0iHHPP1tGLsbEHbgDqViiBD4heNXbt9COEo2DTFsqaDeTwvK9HsTSoQxKWFKrEuPt3R+YFZA1LcLMEsqNSIH3WHhUa+iMMTYfSgYMR1TzN5C4spKJfV+khUrhwJzguqS7gpdj9CuTwf0+b8rB9Typj1IawCUKdg7e/pn+/8Jr9VterHNRSQhWicxDkMyOgQLQoJe2XLGhaWmHkBBoJiY5uB0Qc7AKXcVz0N92O9gt2Yge4+wHz+KO0NP6JlWB7+IDSSMCAwEAAaOCAdcwggHTMD8GCCsGAQUFBwEBBDMwMTAvBggrBgEFBQcwAYYjaHR0cDovL29jc3AuYXBwbGUuY29tL29jc3AwMy13d2RyMDQwHQYDVR0OBBYEFJGknPzEdrefoIr0TfWPNl3tKwSFMAwGA1UdEwEB/wQCMAAwHwYDVR0jBBgwFoAUiCcXCam2GGCL7Ou69kdZxVJUo7cwggEeBgNVHSAEggEVMIIBETCCAQ0GCiqGSIb3Y2QFBgEwgf4wgcMGCCsGAQUFBwICMIG2DIGzUmVsaWFuY2Ugb24gdGhpcyBjZXJ0aWZpY2F0ZSBieSBhbnkgcGFydHkgYXNzdW1lcyBhY2NlcHRhbmNlIG9mIHRoZSB0aGVuIGFwcGxpY2FibGUgc3RhbmRhcmQgdGVybXMgYW5kIGNvbmRpdGlvbnMgb2YgdXNlLCBjZXJ0aWZpY2F0ZSBwb2xpY3kgYW5kIGNlcnRpZmljYXRpb24gcHJhY3RpY2Ugc3RhdGVtZW50cy4wNgYIKwYBBQUHAgEWKmh0dHA6Ly93d3cuYXBwbGUuY29tL2NlcnRpZmljYXRlYXV0aG9yaXR5LzAOBgNVHQ8BAf8EBAMCB4AwEAYKKoZIhvdjZAYLAQQCBQAwDQYJKoZIhvcNAQEFBQADggEBAA2mG9MuPeNbKwduQpZs0+iMQzCCX+Bc0Y2+vQ+9GvwlktuMhcOAWd/j4tcuBRSsDdu2uP78NS58y60Xa45/H+R3ubFnlbQTXqYZhnb4WiCV52OMD3P86O3GH66Z+GVIXKDgKDrAEDctuaAEOR9zucgF/fLefxoqKm4rAfygIFzZ630npjP49ZjgvkTbsUxn/G4KT8niBqjSl/OnjmtRolqEdWXRFgRi48Ff9Qipz2jZkgDJwYyz+I0AZLpYYMB8r491ymm5WyrWHWhumEL1TKc3GZvMOxx6GUPzo22/SGAGDDaSK+zeGLUR2i0j0I78oGmcFxuegHs5R0UwYS/HE6gwggQiMIIDCqADAgECAggB3rzEOW2gEDANBgkqhkiG9w0BAQUFADBiMQswCQYDVQQGEwJVUzETMBEGA1UEChMKQXBwbGUgSW5jLjEmMCQGA1UECxMdQXBwbGUgQ2VydGlmaWNhdGlvbiBBdXRob3JpdHkxFjAUBgNVBAMTDUFwcGxlIFJvb3QgQ0EwHhcNMTMwMjA3MjE0ODQ3WhcNMjMwMjA3MjE0ODQ3WjCBljELMAkGA1UEBhMCVVMxEzARBgNVBAoMCkFwcGxlIEluYy4xLDAqBgNVBAsMI0FwcGxlIFdvcmxkd2lkZSBEZXZlbG9wZXIgUmVsYXRpb25zMUQwQgYDVQQDDDtBcHBsZSBXb3JsZHdpZGUgRGV2ZWxvcGVyIFJlbGF0aW9ucyBDZXJ0aWZpY2F0aW9uIEF1dGhvcml0eTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAMo4VKbLVqrIJDlI6Yzu7F+4fyaRvDRTes58Y4Bhd2RepQcjtjn+UC0VVlhwLX7EbsFKhT4v8N6EGqFXya97GP9q+hUSSRUIGayq2yoy7ZZjaFIVPYyK7L9rGJXgA6wBfZcFZ84OhZU3au0Jtq5nzVFkn8Zc0bxXbmc1gHY2pIeBbjiP2CsVTnsl2Fq/ToPBjdKT1RpxtWCcnTNOVfkSWAyGuBYNweV3RY1QSLorLeSUheHoxJ3GaKWwo/xnfnC6AllLd0KRObn1zeFM78A7SIym5SFd/Wpqu6cWNWDS5q3zRinJ6MOL6XnAamFnFbLw/eVovGJfbs+Z3e8bY/6SZasCAwEAAaOBpjCBozAdBgNVHQ4EFgQUiCcXCam2GGCL7Ou69kdZxVJUo7cwDwYDVR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBQr0GlHlHYJ/vRrjS5ApvdHTX8IXjAuBgNVHR8EJzAlMCOgIaAfhh1odHRwOi8vY3JsLmFwcGxlLmNvbS9yb290LmNybDAOBgNVHQ8BAf8EBAMCAYYwEAYKKoZIhvdjZAYCAQQCBQAwDQYJKoZIhvcNAQEFBQADggEBAE/P71m+LPWybC+P7hOHMugFNahui33JaQy52Re8dyzUZ+L9mm06WVzfgwG9sq4qYXKxr83DRTCPo4MNzh1HtPGTiqN0m6TDmHKHOz6vRQuSVLkyu5AYU2sKThC22R1QbCGAColOV4xrWzw9pv3e9w0jHQtKJoc/upGSTKQZEhltV/V6WId7aIrkhoxK6+JJFKql3VUAqa67SzCu4aCxvCmA5gl35b40ogHKf9ziCuY7uLvsumKV8wVjQYLNDzsdTJWk26v5yZXpT+RN5yaZgem8+bQp0gF6ZuEujPYhisX4eOGBrr/TkJ2prfOv/TgalmcwHFGlXOxxioK0bA8MFR8wggS7MIIDo6ADAgECAgECMA0GCSqGSIb3DQEBBQUAMGIxCzAJBgNVBAYTAlVTMRMwEQYDVQQKEwpBcHBsZSBJbmMuMSYwJAYDVQQLEx1BcHBsZSBDZXJ0aWZpY2F0aW9uIEF1dGhvcml0eTEWMBQGA1UEAxMNQXBwbGUgUm9vdCBDQTAeFw0wNjA0MjUyMTQwMzZaFw0zNTAyMDkyMTQwMzZaMGIxCzAJBgNVBAYTAlVTMRMwEQYDVQQKEwpBcHBsZSBJbmMuMSYwJAYDVQQLEx1BcHBsZSBDZXJ0aWZpY2F0aW9uIEF1dGhvcml0eTEWMBQGA1UEAxMNQXBwbGUgUm9vdCBDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAOSRqQkfkdseR1DrBe1eeYQt6zaiV0xV7IsZid75S2z1B6siMALoGD74UAnTf0GomPnRymacJGsR0KO75Bsqwx+VnnoMpEeLW9QWNzPLxA9NzhRp0ckZcvVdDtV/X5vyJQO6VY9NXQ3xZDUjFUsVWR2zlPf2nJ7PULrBWFBnjwi0IPfLrCwgb3C2PwEwjLdDzw+dPfMrSSgayP7OtbkO2V4c1ss9tTqt9A8OAJILsSEWLnTVPA3bYharo3GSR1NVwa8vQbP4++NwzeajTEV+H0xrUJZBicR0YgsQg0GHM4qBsTBY7FoEMoxos48d3mVz/2deZbxJ2HafMxRloXeUyS0CAwEAAaOCAXowggF2MA4GA1UdDwEB/wQEAwIBBjAPBgNVHRMBAf8EBTADAQH/MB0GA1UdDgQWBBQr0GlHlHYJ/vRrjS5ApvdHTX8IXjAfBgNVHSMEGDAWgBQr0GlHlHYJ/vRrjS5ApvdHTX8IXjCCAREGA1UdIASCAQgwggEEMIIBAAYJKoZIhvdjZAUBMIHyMCoGCCsGAQUFBwIBFh5odHRwczovL3d3dy5hcHBsZS5jb20vYXBwbGVjYS8wgcMGCCsGAQUFBwICMIG2GoGzUmVsaWFuY2Ugb24gdGhpcyBjZXJ0aWZpY2F0ZSBieSBhbnkgcGFydHkgYXNzdW1lcyBhY2NlcHRhbmNlIG9mIHRoZSB0aGVuIGFwcGxpY2FibGUgc3RhbmRhcmQgdGVybXMgYW5kIGNvbmRpdGlvbnMgb2YgdXNlLCBjZXJ0aWZpY2F0ZSBwb2xpY3kgYW5kIGNlcnRpZmljYXRpb24gcHJhY3RpY2Ugc3RhdGVtZW50cy4wDQYJKoZIhvcNAQEFBQADggEBAFw2mUwteLftjJvc83eb8nbSdzBPwR+Fg4UbmT1HN/Kpm0COLNSxkBLYvvRzm+7SZA/LeU802KI++Xj/a8gH7H05g4tTINM4xLG/mk8Ka/8r/FmnBQl8F0BWER5007eLIztHo9VvJOLr0bdw3w9F4SfK8W147ee1Fxeo3H4iNcol1dkP1mvUoiQjEfehrI9zgWDGG1sJL5Ky+ERI8GA4nhX1PSZnIIozavcNgs/e66Mv+VNqW2TAYzN39zoHLFbr2g8hDtq6cxlPtdk2f8GHVdmnmbkyQvvY1XGefqFStxu9k0IkEirHDx22TZxeY8hLgBdQqorV2uT80AkHN7B1dSExggHLMIIBxwIBATCBozCBljELMAkGA1UEBhMCVVMxEzARBgNVBAoMCkFwcGxlIEluYy4xLDAqBgNVBAsMI0FwcGxlIFdvcmxkd2lkZSBEZXZlbG9wZXIgUmVsYXRpb25zMUQwQgYDVQQDDDtBcHBsZSBXb3JsZHdpZGUgRGV2ZWxvcGVyIFJlbGF0aW9ucyBDZXJ0aWZpY2F0aW9uIEF1dGhvcml0eQIIDutXh+eeCY0wCQYFKw4DAhoFADANBgkqhkiG9w0BAQEFAASCAQATeQCUfjTFSFrao9oW5bu4xOo3KZDX7HB9JeKQSFovLGFcL4mKUh8TGUr52FiHAQhDitMYV/kDC/gASM8P1O23wBfgP0+RMzFfX3BKg4ZRHnA4xpSmZKb0rRZCn5PW6cJD94Qq1NoCBWfxpA7PPdy8DOBb2RkHg17TsVig5IG5ehBNcoMvFvUYu1j/l+m2YTgu5n+4c2Sur48ug8B2uVwE3MdvfYf6IoAbOcYfJ/Ypc0asCsip5EH6frgfw5h+To+pitcgPShUGoJ05tokbmJYK65v8R61D3dAcJMXnk5m78wlrcfvid6vZdhAdvYEBhZvuw+mjlaYxfYlLWuQL19k\"}";

    #endregion

    #region Toast

    private IEnumerator PrintToast(int duration) {
        SetToastOpacity(1);
        yield return new WaitForSeconds(duration);
        SetToastOpacity(0);
        coroutine = null;
    }

    private void SetToastOpacity(float opacity) {
        var color = _toast.color;
        color.a = opacity;
        _toast.color = color;
    }

    private void ShowToast(string message) {
        _toast.text = message;

        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(PrintToast(3));
    }

    #endregion
}
