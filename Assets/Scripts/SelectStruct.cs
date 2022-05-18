namespace Graph_struct
{
    public struct SelectStruct
    {
        /*public SelectStruct()
        {
            wantSelect = false;
            selection = Selection.none;
        }*/
        public SelectStruct(Selection selected)
        {
            selection = selected;
            wantSelect = true;
        }

        public bool wantSelect;

        public Selection selection;
    }

    public enum Selection
    {
        none,
        start,
        finish,
    }
}