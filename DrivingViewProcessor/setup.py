from setuptools import setup, find_packages
from distutils.util import convert_path

main_ns = {}
ver_path = convert_path('fieldmapp/version.py')
with open(ver_path) as ver_file:
    exec(ver_file.read(), main_ns)

with open('readme.md') as f:
    readme = f.read()

with open('LICENSE') as f:
    license = f.read()

with open('requirements.txt') as f:
    req = f.read().split("\n")

setup(
    name='fieldmapp',
    version=main_ns['_version'],
    description='FieldMApp post processor',
    long_description=readme,
    author='Eric Krueger',
    author_email='eric.krueger@dlr.de',
    url='https://github.com/',
    license=license,
    packages=find_packages(exclude=('tests', 'docs', 'data', 'dvp'), include=["fieldmapp", "fieldmapp.*"]),
    install_requires=req
)
