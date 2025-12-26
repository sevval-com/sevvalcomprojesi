using Microsoft.AspNetCore.Mvc;
using Sevval.Domain.Entities;
using sevvalemlak.Dto;
using sevvalemlak.Models;

namespace sevvalemlak.csproj.Models
{
    public class GununIlanViewModel : Controller
    {
        public class GununIlanVieewModel
        {
            public List<GununIlanModel> GununIlanlar { get; set; }
            public TumIlanlarDTO TumIlanlar { get; set; }
        }

    }
}
