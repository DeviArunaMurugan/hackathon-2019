using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Flutter_Publish_Utility
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        ViewModel viewModel;
        Frame mainFrame;
        public LoginPage()
        {
            InitializeComponent();
            viewModel = new ViewModel();
            mainFrame = new Frame();
            this.DataContext = viewModel;
            mainFrame.Content = this.border;

        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            Model currentUser = null;
            foreach (var user in viewModel.Userlists)
            {
                if (loginText.Text == user.Username)
                {
                    if (passwordText.Text == user.Password)
                    {
                        currentUser = user;
                        nameLayout.HasError = false;
                        NavigationWindow window = new NavigationWindow();
                        window.Source = new Uri("UtilityPage.xaml", UriKind.Relative);
                        window.Show();
                        //  this.mainFrame.Navigate(new Uri("UtilityPage.xaml", UriKind.Relative));

                    }
                    else
                    {
                        nameLayout.HasError = true;
                    }
                    break;

                }
            }
            if (currentUser == null)
            {
                nameLayout.HasError = true;
                passwordLayout.HasError = true;
            }
            else
            {
                passwordLayout.HasError = false;
            }
        }

     
        private void LoginText_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (loginText.Text == "")
            {
                nameLayout.HasError = false;
            }
        }

        private void PasswordText_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (passwordText.Text == "")
            {
                passwordLayout.HasError = false;
            }
        }

        private void LoginText_LostFocus(object sender, RoutedEventArgs e)
        {
            var hasError = false;
            foreach(var user in viewModel.Userlists)
            {
                if( user.Username == loginText.Text)
                {
                    hasError = false;
                    break;
                }
            }

            if(!string.IsNullOrEmpty(loginText.Text) && hasError)
            {
                nameLayout.HasError = hasError;
            }
           
        }

        private void PasswordText_LostFocus(object sender, RoutedEventArgs e)
        {
            var hasError = false;
            foreach (var user in viewModel.Userlists)
            {
                if (!string.IsNullOrEmpty(passwordText.Text) && user.Username == passwordText.Text)
                {
                    hasError = false;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(passwordText.Text) && hasError)
            {
               passwordLayout.HasError = hasError;
            }
        }
    }

    /// <summary>
    /// Represents the model class
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }
    }

    public class ViewModel
    {
        public ObservableCollection<Model> Userlists { get; set; }
        public ViewModel()
        {
            Userlists = new ObservableCollection<Model>();
            Userlists.Add(new Model() { Username = "a", Password = "a" });
            Userlists.Add(new Model() { Username = "deviaruna.murugan@gmail.com", Password = "deviaruna@123" });
            Userlists.Add(new Model() { Username = "nandhini.ravichandran@gmail.com", Password = "nandhini@123" });
            Userlists.Add(new Model() { Username = "pavitra.ramachandran@gmail.com", Password = "pavitra@123" });
            Userlists.Add(new Model() { Username = "dharanidharan@gmail.com", Password = "dharani@123" });
            Userlists.Add(new Model() { Username = "ashok.k@gmail.com", Password = "ashokk@123" });
        }
    }
}
