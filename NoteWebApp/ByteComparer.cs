namespace NoteWebApp {
    public class ByteComparer : IComparer<byte> {
        public int Compare(byte x, byte y) {
            return x.CompareTo(y);
        }
    }
}
