using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class PagingResult<T> {
        public int Count { get; set; }
        public List<T> Value { get; set; }
    }
}
