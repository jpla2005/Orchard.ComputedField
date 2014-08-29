Orchard.ComputedField
=====================

A simple select file type that can be filled out with values from an external service.

Usage
=====

1. Install the module in your orchard site.
2. Create a field of type ComputedModule in one of yours content definitions.
3. Configure the field.
4. Enjoy.

Configuration
=============

The most important configuration field is Service url. All you need is put the url of a service that returns a list of object with Text and Value properties. It doesn't matter the name of your class if it contains these two properties in their members.

The service url can be an absolute url pointing to service or a relative one (starting with ~/) if your service is part of your orchard site.