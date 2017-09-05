// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ScopeViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Название")]
        [Required]
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool Checked { get; set; }

        public ScopeViewModel()
        {
        }

        public ScopeViewModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
