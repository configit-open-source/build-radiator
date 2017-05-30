namespace Configit.BuildRadiator.Model {
  public abstract class Tile {

    public int Id { get; set; }
    public int Order { get; set; }
    public string Type => this.GetType().ToString();
    public string Title { get; set; }
    public int ColumnSpan { get; set; }
    public int RowSpan { get; set; }
    public bool Error { get; set; }

    protected Tile() {
      ColumnSpan = 1;
      RowSpan = 1;
    }
  }
}