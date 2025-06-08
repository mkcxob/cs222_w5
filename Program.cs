using System.Xml.Serialization;

[XmlInclude(typeof(Circle))]
[XmlInclude(typeof(Rectangle))]
public abstract class Shape
{
    public string Color { get; set; }
    public abstract double Area { get; }
}

public class Circle : Shape
{
    public double Radius { get; set; }
    public override double Area => Math.PI * Radius * Radius;
}

public class Rectangle : Shape
{
    public double Height { get; set; }
    public double Width { get; set; }
    public override double Area => Height * Width;
}

class Program
{
	static void Main()
	{
		var listOfShapes = new List<Shape>
		{
			new Circle { Color = "Red", Radius = 2.5 },
			new Rectangle { Color = "Blue", Height = 20.0, Width = 10.0 },
			new Circle { Color = "Green", Radius = 8.0 },
			new Circle { Color = "Purple", Radius = 12.3 },
			new Rectangle { Color = "Blue", Height = 45.0, Width = 18.0 }
		};

		string fileName = "shapes.xml";
		var serializer = new XmlSerializer(typeof(List<Shape>));

		using (var file = File.Create(fileName))
		{
			serializer.Serialize(file, listOfShapes);
		}

		List<Shape> loadedShapesXml;
		using (var file = File.OpenRead(fileName))
		{
			loadedShapesXml = serializer.Deserialize(file) as List<Shape>;
		}

		Console.WriteLine("Loading shapes from XML:");
		foreach (Shape item in loadedShapesXml)
		{
			Console.WriteLine("{0} is {1} and has an area of {2:N2}", item.GetType().Name, item.Color, item.Area);
		}
	}
}
