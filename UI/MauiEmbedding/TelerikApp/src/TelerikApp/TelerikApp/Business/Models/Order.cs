using System.Xml.Serialization;

#if HAS_TELERIK
using Telerik.Maui.Controls.Compatibility.Common.DataAnnotations;
#endif

namespace TelerikApp.Business.Models;

public class Order : ObservableObject
{
    private int orderID;
    private string? shipName;
    private string? customerID;
    private int employeeID;
    private string? employeeImage;
    private DateTime orderDate;
    private DateTime requiredDate;
    private DateTime shippedDate;
    private double shipVia;
    private double freight;
    private string? shipAddress;
    private string? shipCity;
    private string? shipPostalCode;
    private string? shipCountry;
    private string? shipRegion;

    [XmlAttribute(AttributeName = "OrderID")]
#if HAS_TELERIK
	[DisplayOptions (Header = "Order ID", PlaceholderText = "Order ID")]
#endif
    public int OrderID
    {
        get => this.orderID;
        set => this.SetProperty(ref this.orderID, value);
    }

    [XmlAttribute(AttributeName = "ShipName")]
#if HAS_TELERIK
	[DisplayOptions (Header = "Ship Name", PlaceholderText = "Ship Name")]
#endif
    public string? ShipName
    {
        get => this.shipName;
        set => this.SetProperty(ref this.shipName, value);
    }

    [XmlAttribute(AttributeName = "CustomerID")]
#if HAS_TELERIK
	[DisplayOptions (Header = "Customer ID", PlaceholderText = "Customer ID")]
#endif
    public string? CustomerID
    {
        get => this.customerID;
        set => this.SetProperty(ref this.customerID, value);
    }

    [XmlAttribute(AttributeName = "EmployeeID")]
    public int EmployeeID
    {
        get => this.employeeID;
        set => this.SetProperty(ref this.employeeID, value);
    }

    [XmlAttribute(AttributeName = "EmployeeImage")]
    public string? EmployeeImage
    {
        get => this.employeeImage;
        set => this.SetProperty(ref this.employeeImage, value);
    }

    [XmlAttribute(AttributeName = "OrderDate")]
#if HAS_TELERIK
	[DisplayOptions (Header = "Order Date", PlaceholderText = "Order Date")]
#endif
    public DateTime OrderDate
    {
        get => this.orderDate;
        set => this.SetProperty(ref this.orderDate, value);
    }

    [XmlAttribute(AttributeName = "RequiredDate")]
    public DateTime RequiredDate
    {
        get => this.requiredDate;
        set => this.SetProperty(ref this.requiredDate, value);
    }

    [XmlAttribute(AttributeName = "ShippedDate")]
#if HAS_TELERIK
	[DisplayOptions (Header = "Shipped Date", PlaceholderText = "Shipped Date")]
#endif
	public DateTime ShippedDate
    {
        get => this.shippedDate;
        set => this.SetProperty(ref this.shippedDate, value);
    }

    [XmlAttribute(AttributeName = "ShipVia")]
#if HAS_TELERIK
	[DisplayOptions (Header = "Ship Via", PlaceholderText = "Ship Via")]
#endif
	public double ShipVia
    {
        get => this.shipVia;
        set => this.SetProperty(ref this.shipVia, value);
    }

    [XmlAttribute(AttributeName = "Freight")]
#if HAS_TELERIK
	[DisplayOptions (Header = "Freight", PlaceholderText = "Freight")]
#endif
    public double Freight
    {
        get => this.freight;
        set => this.SetProperty(ref this.freight, value);
    }

    [XmlAttribute(AttributeName = "ShipAddress")]
    public string? ShipAddress
    {
        get => this.shipAddress;
        set => this.SetProperty(ref this.shipAddress, value);
    }

    [XmlAttribute(AttributeName = "ShipCity")]
    public string? ShipCity
    {
        get => this.shipCity;
        set => this.SetProperty(ref this.shipCity, value);
    }

    [XmlAttribute(AttributeName = "ShipPostalCode")]
    public string? ShipPostalCode
    {
        get => this.shipPostalCode;
        set => this.SetProperty(ref this.shipPostalCode, value);
    }

    [XmlAttribute(AttributeName = "ShipCountry")]
    public string? ShipCountry
    {
        get => this.shipCountry;
        set => this.SetProperty(ref this.shipCountry, value);
    }

    [XmlAttribute(AttributeName = "ShipRegion")]
    public string? ShipRegion
    {
        get => this.shipRegion;
        set => this.SetProperty(ref this.shipRegion, value);
    }
}
