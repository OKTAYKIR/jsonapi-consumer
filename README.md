# JsonApi Client
![Hits](https://hitcounter.pythonanywhere.com/count/tag.svg?url=https://github.com/OKTAYKIR/jsonapi-consumer)
![GitHub issues](https://img.shields.io/github/issues/OKTAYKIR/jsonapi-consumer)
![Build Status](https://github.com/OKTAYKIR/jsonapi-consumer/workflows/CI/badge.svg?branch=master) 
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](#contributing)
[![nuget](https://img.shields.io/nuget/v/jsonapi-consumer)](https://www.nuget.org/packages/jsonapi-consumer/)

Client framework for consuming JSONAPI web services based on the [JSON API standard](http://jsonapi.org/)

## :package: Installation
jsonapi-consumer is available on [NuGet](https://www.nuget.org/packages/jsonapi-consumer/). 

```sh
dotnet add package jsonapi-consumer
```

## 🚀 Usage

#### Create HttpGet request with array response object
```c#
Response<User[]> response = JsonApiConsumer.Get<User>(
    baseURI: ABSOLUTE_URL,
    path: RELATIVE_URI,
    query: new Dictionary<string, string>() { { "FirstName", "Oktay" }, { "LastName", "Kır" } },
    headers: new Dictionary<string, string>() { { HEADER_API_KEY, HEADER_API_KEY_VALUE } } );
```
#### Create HttpGet request with single response object
```c#
Response<User> response = JsonApiConsumer.GetById<User>(
    id: "c833cbbf-7c81-4d30-b11a-88cf1c990b9c",
    baseURI: ABSOLUTE_URL,
    path: RELATIVE_URI,
    query: new Dictionary<string, string>() { { "FirstName", "Oktay" }, { "LastName", "Kır" } },
    headers: new Dictionary<string, string>() { { HEADER_API_KEY, HEADER_API_KEY_VALUE } } );
```

#### Create HttpPost request
```c#
var user = new User()
{
    id = "c833cbbf-7c81-4d30-b11a-88cf1c990b9c";
    FirstName = "Oktay"; 
    LastName="Kır";
}

Response<CreateUserResponse> response = JsonApiConsumer.Create<User, CreateUserResponse>(
    model: user,
    baseURI: ABSOLUTE_URL,
    path: RELATIVE_URI,
    headers: new Dictionary<string, string>() { { HEADER_API_KEY, HEADER_API_KEY_VALUE } } );
```

#### Post a file
```c#
Response<PostFileResponse> response = JsonApiConsumer.PostFile<PostFileResponse>(
    fileName: "filename",
    data: new byte[],
    baseURI: ABSOLUTE_URL,
    path: RELATIVE_URI,
    headers: new Dictionary<string, string>() { { HEADER_API_KEY, HEADER_API_KEY_VALUE } } );
```

#### Create HttpPut request
```c#
var user = new User()
{
    FirstName = "Oktay"; 
    LastName="Kır";
}

Response<UpdateUserResponse> response = JsonApiConsumer.Update<User, UpdateUserResponse>(
    id: "c833cbbf-7c81-4d30-b11a-88cf1c990b9c",
    model: user,
    baseURI: ABSOLUTE_URL,
    path: RELATIVE_URI,
    headers: new Dictionary<string, string>() { { HEADER_API_KEY, HEADER_API_KEY_VALUE } } );
```

#### Create HttpPatch request
```c#
var user = new User()
{
    FirstName = "CC"; 
}

Response<PatchUserResponse> response = JsonApiConsumer.Patch<User, PatchUserResponse>(
    id: "c833cbbf-7c81-4d30-b11a-88cf1c990b9c",
    model: user,
    baseURI: ABSOLUTE_URL,
    path: RELATIVE_URI,
    headers: new Dictionary<string, string>() { { HEADER_API_KEY, HEADER_API_KEY_VALUE } } );
```

#### Create HttpDelete request
```c#
Response<DeleteUserResponse> response = JsonApiConsumer.Delete<DeleteUserResponse>(
    id: "c833cbbf-7c81-4d30-b11a-88cf1c990b9c",
    baseURI: ABSOLUTE_URL,
    path: RELATIVE_URI,
    headers: new Dictionary<string, string>() { { HEADER_API_KEY, HEADER_API_KEY_VALUE } } );
```

#### Response class definition
```c#
public class Response<T>
{
	public DocumentRoot<T> documentRoot { get; internal set; }
	public HttpStatusCode httpStatusCode { get; internal set; }
	public Error error { get; set; }
	public bool IsSuccess { get; internal set; }
}
```

## ✨ Contributors
![GitHub Contributors Image](https://contrib.rocks/image?repo=OKTAYKIR/jsonapi-consumer)

## 🤝 Contributing
1. Fork it ( https://github.com/OKTAYKIR/jsonapi-consumer/fork )
2. Create your feature branch (`git checkout -b my-new-feature`)
3. Commit your changes (`git commit -am 'Add some feature'`)
4. Push to the branch (`git push origin my-new-feature`)
5. Create a new Pull Request

## Show your support
Please ⭐️ this repository if this project helped you!

## 📝 License
MIT License
