using NEbml.Core;

namespace NEbml.MkvTitleEdit.Matroska
{
	public static class ReaderExtensions
	{
		public static bool LocateElement(this EbmlReader reader, ElementDescriptor descriptor)
		{
			while (reader.ReadNext())
			{
				if (reader.ElementId == descriptor.Identifier)
				{
					return true;
				}
			}

			return false;
		}
	}
}