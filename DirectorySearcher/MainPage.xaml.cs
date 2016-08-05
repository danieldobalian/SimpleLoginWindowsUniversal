using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleLogin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        // The Tenant is the name of the Azure AD tenant in which this application is registered.
        // The AAD Instance is the instance of Azure, for example public Azure or Azure China.
        // The Authority is the sign-in URL of the tenant.
        //

        const string tenant = "ddobalianoutlook.onmicrosoft.com";
        const string clientId = "ddef4ea9-bf82-4179-bba6-b5bdd46c6f9e";
        const string aadInstance = "https://login.microsoftonline.com/{0}";
        static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        const string graphResourceId = "https://graph.microsoft.com/";
        const string graphEndpoint = "https://graph.microsoft.com/";

        private HttpClient httpClient = new HttpClient();
        private AuthenticationContext authContext = null;
        private Uri redirectURI = null;

        public MainPage()
        {
            redirectURI = Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri();

            // TODO: Initialize the Authentication Context
            authContext = new AuthenticationContext(authority);
            this.InitializeComponent();
        }

        // clear the token cache
        private void RemoveAppBarButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void ShowAuthError(string message)
        {
            MessageDialog dialog = new MessageDialog(message, "Sorry, an error occurred while signing you in.");
            await dialog.ShowAsync();
        }

        private async void SignIn(object sender, RoutedEventArgs e)
        {
            AuthenticationResult result = null;
            try
            {
                result = await authContext.AcquireTokenAsync(graphResourceId, clientId, redirectURI, new PlatformParameters(PromptBehavior.Always, false));

                // Reset UI elements
                SignOutButton.Visibility = Visibility.Visible;
                SignInButton.Visibility = Visibility.Collapsed;
                ActiveUser.Text = result.UserInfo.DisplayableId;
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode != "authentication_canceled")
                {
                    ShowAuthError(string.Format("If the error continues, please contact your administrator.\n\nError: {0}\n\nError Description:\n\n{1}", ex.ErrorCode, ex.Message));
                }
                return;
            }
        }

        private void SignOut(object sender, RoutedEventArgs e)
        {
            // TODO: Sign the user out by clearing the token cache
            authContext.TokenCache.Clear();

            // Reset UI elements
            SignOutButton.Visibility = Visibility.Collapsed;
            SignInButton.Visibility = Visibility.Visible;
            ActiveUser.Text = "Signed out.";
        }

        //private async void Search(object sender, RoutedEventArgs e)
        //{
        //    string graphRequest = String.Format(CultureInfo.InvariantCulture, "{0}{1}/v{2}/me/, graphEndpoint, tenant, graphApiVersion);
        //    HttpResponseMessage response = await httpClient.GetAsync(new Uri(graphRequest));

        //    // Add the access token to the Authorization Header of the call to the Graph API, and call the Graph API.
        //    httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("Bearer", result.AccessToken);

        //    // Update the Page UI to represent the signed in user
        //    ActiveUser.Text = result.UserInfo.DisplayableId;

        //    if (response.StatusCode == HttpStatusCode.Unauthorized)
        //    {
        //        // If the Graph API returned a 401, user may need to sign in again.
        //        MessageDialog dialogg = new MessageDialog("Sorry, you don't have access to the Graph API.  Please sign-in again.");
        //        await dialogg.ShowAsync();
        //        SignOut();
        //        return;
        //    }
        //    string content = await response.Content.ReadAsStringAsync();
        //}
    }
}
