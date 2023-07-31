using Leavetown.Client.Clients;
using Leavetown.Client.Models.Projections;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Forms.Contracts
{
    public interface IFormComponent<T>
    {
        [Parameter] public string? Title { get; set; }
        [Parameter] public T Data { get; set; }

    }
}
