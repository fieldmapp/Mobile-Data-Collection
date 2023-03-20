# README

## Build and register the FieldMApp python package

- update the version number `fieldmapp/version.py`

- build the package with setuptools `python setup.py bdist_wheel`
  - This will create a wheel distribution. The wheel distribution contains a compiled
    version of your package that can be installed on any platform.
- **Optional** Upload your package to the GitLab Package Registry using the twine utility by
  running `python3 twine upload --repository gitlab dist/*` This will upload the distributions 
  to the GitLab Package Registry --> This is optional because normally a new package is registered autom. through the 
  gitlab CI
  - remember to create `.pypirc` in your home folder with a content like this ex.:
  ```ini
    [distutils]
    index-servers = gitlab

    [gitlab]
    repository = https://gitlab.com/api/v4/projects/<project-id>/packages/pypi
    username = <token_name>
    password = <access_token>
  ```
  - create access token in order to be able to register packages manually
  - https://docs.gitlab.com/ee/user/packages/pypi_repository/index.html#authenticate-with-a-ci-job-token


  ## Install Package from registry

  - install from `requirements.txt`
    - requirements.txt:
    ```
    --extra-index-url https://<toke_name>:<token>@gitlab.com/api/v4/projects/<project-id>/packages/pypi/simple  
    <package-name>==<version>
    ```
  - install with `pip install -r requirements.txt`