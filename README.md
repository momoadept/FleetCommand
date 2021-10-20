# FleetCommand

This is my scripting repo for space engineers. The fun stuff is AdeptOS, which is the framework I designed for scripting. Features include:
- Modular architercure with basic dependency injection (core)
  - Keep your reusable code in isolated modules and pack them together in one script according to your needs
  - Link your modules via interfaces and DI
- Asynchronous runtime (core)
  - This is like Promises in JS
  - A lot of cool magic for async scheduling with as good of a syntax that I could implement
  - With performance in mind!
- Persistance (core)
  - Basic state management
  - String serialization for custom types and collections
- Command syntax (core)
  - Call any function on your modules from any triggers (PB argument, timer, button) using a simple syntax
  - Pass arguments
- Sequencing (core)
  - Manage complex sequences of async actions
  - Do step by step, stop at any time, handle exceptions
  - Combine simple sequences to create more complex ones
  - Built on top of Promises
- Logging (core)
  - You can log stuff, that's it
  - But you can also build your own loggers
- Remote procedure call (optional)
  - Seamlessly divide your script across multiple programmable blocks to bypass code size limit and/or better manage performance
  - Asynchronously executed functions with returned results, and a caller still sees only a plain interface
  - Make all PBs on your ship a part of a single distributed system with a required degree of redundancy

Full guide coming soon
