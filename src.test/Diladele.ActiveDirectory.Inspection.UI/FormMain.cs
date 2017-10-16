using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diladele.ActiveDirectory.Inspection.UI
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // load storage
            _storage = StorageFactory.LoadFromDisk();

            // start harvester
            _harvester = new Harvester(_storage);

            // and start listener
            _listener = new Listener(_storage);

            // and start the timer
            _timerRefresh.Start();            
        }

        private Storage   _storage;
        private Harvester _harvester;
        private Listener  _listener;

        private void _timerRefresh_Tick(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void _menuRefreshList_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void RefreshList()
        {
            List<Address> list = new List<Address>();
            {
                if(!_storage.Dump(out list))
                    return;
            }

            _statusLabelCount.Text = string.Format("Address List Count: {0}", list.Count);

            _lstStorage.Items.Clear();

            foreach (var item in list)
            {
                ListViewItem lvi = _lstStorage.Items.Add(item.IP.ToString());

                lvi.SubItems.Add(item.DnsHostName);
                lvi.SubItems.Add(item.Users.Count.ToString());

                if(item.Users.Count > 0)
                    lvi.SubItems.Add(string.Format(@"{0}\{1}", item.Users[0].Domain, item.Users[0].Name));
                if (item.Users.Count > 1)
                    lvi.SubItems.Add(string.Format(@"{0}\{1}", item.Users[1].Domain, item.Users[1].Name));
            }





            
            

            // create listener
            //using(var harvester = new Harvester(storage))
            //{

            //}

        }

        
    }
}
