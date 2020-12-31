using System;

namespace Matroska
{
	/// <summary>
	/// Represents EBML element info as defined in the specdata.xml.
	/// </summary>
	public class EbmlElementInfo
	{
		#region Properties

		/// <summary>
		/// Gets the ID of the element.
		/// </summary>
		public uint ID { get; }

		/// <summary>
		/// Gets the name of the element.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the type of the element.
		/// </summary>
		public EbmlElementType Type { get; }

		/// <summary>
		/// Gets the flags of the element.
		/// </summary>
		public EbmlElementFlags Flags { get; }

		/// <summary>
		/// Gets the level at which the element may occur.
		/// </summary>
		public int Level { get; }

		/// <summary>
		/// Gets a description of the element.
		/// </summary>
		public string Description { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="EbmlElementInfo"/> class.
		/// </summary>
		public EbmlElementInfo(uint id, string name, EbmlElementType type, EbmlElementFlags flags, int level, string description)
		{
			ID = id;
			Name = name;
			Type = type;
			Flags = flags;
			Level = level;
			Description = description;
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return $"{Type} {Name}";
		}

		public bool Equals(EbmlElementInfo other)
		{
			if (other == null)
				return false;

			return other.ID == ID &&
				   other.Name == Name &&
				   other.Type == Type &&
				   other.Flags == Flags &&
				   other.Level == Level &&
				   other.Description == Description;
		}

		public override bool Equals(object obj)
		{
			EbmlElementInfo other = obj as EbmlElementInfo;
			return other != null && Equals(other);
		}

		public override int GetHashCode()
		{
			return unchecked((int)ID);
		}

		#endregion
	}
}
