public class Target
{
    public string Name { get; set; }

    public string Url { get; set; }

    public override string ToString()
    {
        return Name + " : " + Url;
    }
}