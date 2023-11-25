# To-Do-List-App

This project is a to-do list app that converts formatted markdown into list objects that can be viewed, edited, and completed as in a typical to-do list program. Upon the closing of a list, the data is serialized back to markdown so that it is completely human-readable, transferable, and editable in a standard editor.

In addition the this core feature of markdown serialization, it will also support the declaration of object types that are particular to that list. It bridges the gap between the plaintext notes people create to remember information and an SQL database that is too rigid to maintain for personal use.

## Implementation Roadmp

- [x] Design markdown formatting
- [ ] Implement parser to convert markdown into list objects (active)
- [ ] Implement serializer to convert from list objects back into a markdown file
- [ ] Create user interface for viewing and modifying lists
- [ ] Implement functionality for aggregating data across lists and displaying active and scheduled items in one location
- [ ] Implement caching to quickly populate active and scheduled tasks list
- [ ] Implement language intelligence tools that would allow users who prefer managin their lists in a text editor to do so with code completion and syntax highlighting
