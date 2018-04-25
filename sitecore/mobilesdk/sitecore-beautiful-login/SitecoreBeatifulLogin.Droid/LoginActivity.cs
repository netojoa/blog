using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SitecoreBeautifulLogin.Droid.Services;
using System.Threading.Tasks;

namespace SitecoreBeautifulLogin.Droid
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme.Dark")]
    public class LoginActivity : Activity
    {

        private EditText userText;
        private EditText passwordText;
        private Button loginButton;
        private SitecoreService sitecoreService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Login);

            userText = FindViewById<EditText>(Resource.Id.login_user_text);
            passwordText = FindViewById<EditText>(Resource.Id.login_password_text);
            loginButton = FindViewById<Button>(Resource.Id.login_button);

            loginButton.Click += LoginButtonClicked;

            sitecoreService = new SitecoreService();

        }

        private async void LoginButtonClicked(object sender, EventArgs e)
        {
            if (!ValidateLogin())
            {
                OnLoginFailed();
                return;
            }

            loginButton.Enabled = false;

            ProgressDialog progressDialog = new ProgressDialog(this, Resource.Style.AppTheme_Dark_Dialog);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage("Authenticating...");
            progressDialog.Show();

            string login = userText.Text;
            string password = passwordText.Text;
            string message = string.Empty;


            try
            {

                Task<bool> authentication = sitecoreService.Authenticate(login, password);
                bool isAuthenticated = await authentication;

                if (isAuthenticated)
                {
                    OnLoginSuccess();
                }
                else
                {
                    OnLoginFailed();
                    Toast.MakeText(this, "User/Password invalid", ToastLength.Long).Show();
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, string.Format("An exception occurred {0}", ex.Message), ToastLength.Long).Show();
            }
            finally
            {
                progressDialog.Dismiss();

            }

        }

        private void OnLoginSuccess()
        {
            Toast.MakeText(this, "Login success", ToastLength.Long).Show();
            loginButton.Enabled = true;
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        private void OnLoginFailed()
        {
            loginButton.Enabled = true;
        }

        private bool ValidateLogin()
        {
            bool valid = true;

            string user = userText.Text;
            string password = passwordText.Text;

            if (string.IsNullOrEmpty(user))
            {
                userText.Error = "Enter a valid user";
                valid = false;
            }
            else
            {
                userText.Error = null;
            }

            if (string.IsNullOrEmpty(password))
            {
                passwordText.Error = "Enter your password";
                valid = false;
            }
            else
            {
                passwordText.Error = null;
            }

            return valid;
        }
    }
}