using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Tasks;

namespace oldrotarydial
{
    public partial class ContactView : UserControl
    {
        public ContactView()
        {
            InitializeComponent();
            
        }

        private void Image_Tap(object sender, GestureEventArgs e)
        {
            PhoneCallTask phonecalltask = new PhoneCallTask();
            phonecalltask.PhoneNumber = this.contactnumber.Text;
            phonecalltask.Show();
        }
    }
}
