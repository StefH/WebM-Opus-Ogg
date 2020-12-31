Matroska / EBML library
=======================

This is a small, lightweight EBML and Matroska (just the format itself) implementation in C#. It can read and write EBML documents from a stream.

Examples
--------

Read MKV file and list child elements of EBML header element.
```
using (var file = new FileStream("test.mkv", FileMode.Open, FileAccess.Read))
{
	EbmlElement ebml = EbmlElement.ReadElement(file);
	foreach (EbmlElement element in EbmlElement.ReadElements(ebml))
	{
		Console.WriteLine(MatroskaSpecification.Elements[element.ID].Name);
	}
}

```

Writing some EBML.
```
using (var file = new FileStream("test.mkv", FileMode.Create, FileAccess.Write))
{
	// EBML root.
	EbmlElement ebml = EbmlElement.WriteElement(file, MatroskaSpecification.EBML.ID, 15);

	// Version 1.
	EbmlElement versionElement = EbmlElement.WriteElement(ebml, MatroskaSpecification.EBMLVersion.ID, 1);
	versionElement.WriteByte(1);

	// DocType "matroska".
	EbmlElement docTypeElement = EbmlElement.WriteElement(ebml, MatroskaSpecification.DocType.ID, 8);
	docTypeElement.Write(Encoding.ASCII.GetBytes("matroska"), 0, 8);
}
```

License
-------

MIT


