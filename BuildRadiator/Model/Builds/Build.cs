using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Configit.BuildRadiator.Controllers;

namespace Configit.BuildRadiator.Model.Builds {
  public class Build {
    public int Id { get; set; }
    public string Name { get; set; }
    public string BranchName { get; set; }

    [NotMapped]
    public BuildStatus Status { get; set; }
    [NotMapped]
    public string StatusText { get; set; }
    [NotMapped]
    public string StatusSubText { get; set; }
    [NotMapped]
    public bool PreviouslyFailing { get; set; }

    [NotMapped]
    public DateTime Start { get; set; }
    [NotMapped]
    public DateTime End { get; set; }
    [NotMapped]
    public int PercentComplete { get; set; }
    [NotMapped]
    public string Investigator { get; set; }
    [NotMapped]
    public ICollection<string> Committers { get; set; }

    public TileController.BuildServer Server { get; set; }

    public Build() {
      Committers = new List<string>();
    }
  }
}