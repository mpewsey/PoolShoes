# Unity Package Template

A repository template for Unity packages.

## Setup

* Fork this repository and use it as a template when creating a new repository.
* Create a blank Unity project, then clone your repository under its `Packages` directory.
* The package will display under `Packages` in the Unity file explorer and can be edited directly from your blank Unity project.
* Remember to update the `package.json` file based on your project.

## Documentation Generation

To enable documentation generation, you must enable read and write permissions for GitHub actions:

1. From the repository, go to `Settings`.
2. Go to the `Actions (General)` settings.
3. Under `Workflow permissions`, select `Read and write permissions` and save the settings.

Once the documentation generation action has been run once, you must deploy the generated `gh-pages` branch.

1. From the repository, got to `Settings`.
2. Go to the `Pages` settings.
3. Under `Build and deployment`, select the `gh-pages` branch root and save the settings.

By default, the documentation is generated from files contained in a `Scripts` directory.
