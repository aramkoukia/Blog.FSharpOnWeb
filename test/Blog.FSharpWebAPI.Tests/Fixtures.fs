module Fixtures

open Blog.FSharpWebAPI
open Blog.FSharpWebAPI.Models
open System
open System.Net
open System.Net.Http
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open Microsoft.Extensions.DependencyInjection
open Microsoft.EntityFrameworkCore;
open Xunit
open DataAccess
open Microsoft.EntityFrameworkCore
    


let createHost() =
    WebHostBuilder()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .Configure(Action<IApplicationBuilder> Blog.FSharpWebAPI.App.configureApp)
        .ConfigureServices(Action<IServiceCollection> Blog.FSharpWebAPI.App.configureServices)


let runTask task =
    task
    |> Async.AwaitTask
    |> Async.RunSynchronously

let get (client : HttpClient) (path : string) =
    path
    |> client.GetAsync
    |> runTask

let createRequest (method : HttpMethod) (path : string) =
    let url = "http://127.0.0.1" + path
    new HttpRequestMessage(method, url)

let makeRequest (client : HttpClient) (request : HttpRequestMessage) =
    use server = new TestServer(createHost())
    use client = server.CreateClient()
    request
    |> client.SendAsync
    |> runTask

let ensureSuccess (response : HttpResponseMessage) =
    if not response.IsSuccessStatusCode
    then response.Content.ReadAsStringAsync() |> runTask |> failwithf "%A"
    else response

let isStatus (code : HttpStatusCode) (response : HttpResponseMessage) =
    Assert.Equal(code, response.StatusCode)
    response

let isOfType (contentType : string) (response : HttpResponseMessage) =
    Assert.Equal(contentType, response.Content.Headers.ContentType.MediaType)
    response

let readText (response : HttpResponseMessage) =
    response.Content.ReadAsStringAsync()
    |> runTask

let shouldEqual expected actual = Assert.Equal(expected, actual)
let shouldNotNull expected = Assert.NotNull(expected)