using OpenZWave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Euricom.IoT.Api.DanaLock
{
    /// <summary>
    /// Container for Nodes
    /// </summary>
    public sealed class Node : INotifyPropertyChanged
    {
        private bool m_isLoading = true;

        private String m_label = "";

        private String m_location = "";

        private String m_manufacturer = "";

        private String m_name = "";

        private String m_product = "";

        public Node(byte nodeId, uint homeId)
        {
            ID = nodeId;
            HomeID = homeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IList<ZWValueID> BasicValues { get; } = new ObservableCollection<ZWValueID>();

        public IList<ZWValueID> ConfigValues { get; } = new ObservableCollection<ZWValueID>();

        public Uri DeviceIcon
        {
            get
            {
                return new Uri($"ms-appx:///DeviceIcons/{GenericType}.png");
            }
        }

        public byte GenericType
        {
            get
            {
                return ZWManager.Instance.GetNodeGeneric(HomeID, ID);
            }
        }

        public bool HasNodeFailed
        {
            get
            {
                return ZWManager.Instance.HasNodeFailed(HomeID, ID);
            }
        }

        /// <summary>
        /// Gets or sets the home identifier.
        /// </summary>
        /// <value>
        /// The home identifier.
        /// </value>
        public UInt32 HomeID { get; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Byte ID { get; }

        public bool IsLoading
        {
            get { return m_isLoading; }
            set { m_isLoading = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public String Label
        {
            get { return m_label; }
            set { m_label = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public String Location
        {
            get { return m_location; }
            set
            {
                if (m_location != value)
                {
                    m_location = value;
                    OnPropertyChanged();
                    ZWManager.Instance.SetNodeLocation(HomeID, ID, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public String Manufacturer
        {
            get { return m_manufacturer; }
            set { if (value != "Unknown: id=0000") { m_manufacturer = value; OnPropertyChanged(); } }
        }

        public string ManufacturerAndProduct
        {
            get
            {
                if (!string.IsNullOrEmpty(Manufacturer) && !string.IsNullOrEmpty(Product))
                {
                    return $"{Manufacturer} - {Product}";
                }
                else if (!string.IsNullOrEmpty(Manufacturer))
                    return Manufacturer;
                else return Product;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public String Name
        {
            get { return m_name; }
            set
            {
                if (m_name != value)
                {
                    m_name = value;
                    OnPropertyChanged();
                    ZWManager.Instance.SetNodeName(HomeID, ID, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        public String Product
        {
            get { return m_product; }
            set
            {
                if (value != "Unknown: type=0000, id=0000")
                {
                    m_product = value; OnPropertyChanged();
                }
            }
        }

        public byte SpecificType
        {
            get
            {
                return ZWManager.Instance.GetNodeSpecific(HomeID, ID);
            }
        }

        public IList<ZWValueID> SystemValues { get; } = new ObservableCollection<ZWValueID>();

        public IList<ZWValueID> UserValues { get; } = new ObservableCollection<ZWValueID>();

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="valueID">The value identifier.</param>
        public void AddValue(ZWValueID valueID)
        {
            IList<ZWValueID> list = GetValues(valueID.Genre);

            var id = list.Where(v => v.CommandClassId == valueID.CommandClassId && v.Id == valueID.Id).FirstOrDefault();
            if (id != null)
            {
                list[list.IndexOf(id)] = valueID;
            }
            else
            {
                list.Add(valueID);
            }
        }

        /// <summary>
        /// Removes the value.
        /// </summary>
        /// <param name="valueID">The value identifier.</param>
        public void RemoveValue(ZWValueID valueID)
        {
            var values = GetValues(valueID.Genre);
            values.Remove(valueID);
        }

        private IList<ZWValueID> GetValues(ZWValueGenre genre)
        {
            if (genre == ZWValueGenre.Basic)
                return BasicValues;
            else if (genre == ZWValueGenre.Config)
                return ConfigValues;
            else if (genre == ZWValueGenre.System)
                return SystemValues;
            else if (genre == ZWValueGenre.User)
                return UserValues;
            return null;
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}