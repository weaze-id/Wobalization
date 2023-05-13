# Wobalization

Wobalization is a powerful tool for managing and serving translations via a REST API. It is built using ASP.NET Core (Minimal API) for the REST API and Blazor WebAssembly (Wasm) for the dashboard. With Wobalization, you can easily localize your applications and streamline the translation process.

## Features

-   REST API: Wobalization provides a robust REST API built using ASP.NET Core (Minimal API). You can manage and serve translations by integrating the API into your applications and automating the translation process.
-   Dashboard: The project includes a user-friendly dashboard built with Blazor WebAssembly (Wasm). It provides an intuitive interface for managing translations, allowing you to create, update, and delete translations, as well as monitor translation progress.
-   Translation Management: Wobalization simplifies the translation management process. You can organize translations into different projects, assign translators, and track the status of each translation.
-   User Management: Wobalization includes a user management feature that allows you to create and manage user accounts. Users can authenticate and access the system based on their assigned roles.
-   Translation Collaboration: Wobalization enables collaboration between translators and reviewers. They can work together on translations, communicate, and make changes collaboratively.
-   Localization Support: Wobalization supports multiple languages and locales, making it easy to translate your applications into different target languages.
-   API Documentation: The project provides comprehensive API documentation, allowing developers to understand and use the REST API effectively.

## Getting Started

To get started with Wobalization, follow these steps:

1. Clone the repository:

```bash
git clone https://github.com/your-username/wobalization.git
```

2. Install the required dependencies:

```bash
cd wobalization
dotnet restore
```

3. Configure the environment variables:

-   Open the appsettings.json file in the root directory.
-   Provide the necessary configuration details, such as database connection strings, API keys, and secret keys.

4. Build and run the application:

```bash
# Run the api server
cd src/cmd/Wobalization.Api
dotnet run

# Run the dashboard
cd src/cmd/Wobalization.Wasm
dotnet run
```

5. Access the dashboard:

Open your web browser and navigate to http://localhost:5000 to access the Wobalization dashboard.

6. Explore the API documentation:

Visit http://localhost:5000/swagger to access the API documentation and learn how to use the Wobalization REST API.

## Contributing

We welcome contributions to improve Wobalization. To contribute, please follow these guidelines:

-   Fork the repository on GitHub.
-   Create a new branch for your feature or bug fix.
-   Make the necessary changes and ensure the codebase is well-tested.
-   Commit your changes and push the branch to your fork.
-   Submit a pull request with a detailed description of your changes.

## License

This project is licensed under the MIT License. Feel free to use, modify, and distribute the software according to the terms of the license.

## Support

If you encounter any issues or have any questions or suggestions, please open an issue on GitHub. We are here to help and improve the project.

Happy Wobalizing!

