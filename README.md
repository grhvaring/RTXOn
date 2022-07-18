## About the Project

`RTXOn` is an open source C# solution for the generation of photorealistic images using raytracing.

It is the result of a team effort of Andrea Mastropasqua and Giacomo Riccardo Hvaring, two Master Degree Students in Physics at the time of the release of `v1.0.0` of this project.

The project is loosely[^1] based on a [University Lab Course][raytracing_course] we attended in the Summer Semester of 2022, which tricked us into learning new coding skills, with the promise of basically making VFX artists out of us by the end of the semester.

Obviously, we did not get any call from Pixar (yet), but we feel confident about saying we moved our VFX artist bar from 0 to some number striclty greater than 0, and learned some C# in the process, among other things.

If you are still reading, we've got a prize for you: a beautiful image rendered with `RTXOn`! Which you can find in the next section.

[^1]: It is basically a rewriting of the Professor's GitHub repository [pytracer].

<h2 id="sneak-peak">Sneak Peak</h2>

With `RTXOn` you can generate an image similar to this one:

<!-- ![demo](https://github.com/grhvaring/RTXOn/tree/document/images/demo.png?raw=true)-->

using the following command:

```
$ ./render demo
```

Do not worry if the syntax is not crystal clear: there's a more [detailed explanation](#usage) on how to use `RTXOn` further down.

The next section explains how to get `RTXOn` up and running on your machine.

## Get Started

What follows is a step-by-step guide to install `RTXOn` and its dependencies.

### 1. Clone the repo

To clone the repository, write the command:

```
$ git clone https://github.com/grhvaring/RTXOn.git
```

The symbol `$` at the beginning of the line indicates that the command is supposed to be fed to the command line.

### 2. Install dependencies

The project uses external libraries, here's how to install them.

**ImageSharp 2.1.0** (used for the conversion between HDR and LDR images) can be installed with the following command:

```
$ dotnet add package SixLabors.ImageSharp --version 2.1.0
```

**CommandLineParser 2.8.0** (manages CL runtime options) can be installed with this command:


```
$ dotnet add package CommandLineParser --version 2.8.0
```

### 3. Compile the code

To build the project run the command:

```
$ dotnet build -c Release
```

The option `-c` (configuration) is usually set to `Debug` by default, switching to `Release` mode makes the execution of the program significantly faster.

### 4. Run the tests

To check that the program behaves as expected, you can run the unit tests using this command:

```
$ dotnet test
```

### 5. All set!

If you arrived here in one piece, congratulations ! You succesfully installed and tested `RTXOn`!

The next section explains how to use it.


<h2 id="usage">Usage</h2>

### Easy route

The fastest way to use `RTXOn` is with the program `render`, a short Bash script which deals with all the nitty-gritty details of the image generation for you.

Before you run it though, you need to make `render` executable if it is not already, and you can do so with this command:

```
$ chmod +x render
```


Now, this is the command to render a 3D scene described in the file `scene.txt`:

```
./render scene
```

**Remark:** `scene` is **not** the complete name of the file, but the name without extension. In this case the complete name would be `scene.txt`, and is assumed to be in the sub-directory `examples`. So, if you create a new 3D scene file, make sure it satisfies these two requirements:

1. It is encoded as a **text** file (ends with `.txt`)
2. It is located in the sub-directory `examples`

If you want to create your own 3D scene, take a look at the syntax of the already existing [examples], and have fun !

This is the *basic* way of running `RTXOn`, for the more *advanced* way, keep reading.

### Bumpy road

If you like get your hands a little dirty, this section is for you: this is how to run `RTXOn` from the command line.

This command shows how to fine-tune the parameters of the image generation with `RTXOn`:


```
$ ./path-to-exe/RTXOn render --input-file examples/pyramid.txt --luminosity 50
```

`path-to-exe` indicates the position of the executable file (`RTXOn`) relative to the working directory. Usually it can be found in `RTXOn/bin/Release/net6.0/`, as depicted in this directory tree:

```
├── RTXOn
│   ├── bin
│   │   └── Release
│   │       └── net6.0
│   │           ├── RTXOn
│   │           ...
│   ├── obj
│   │   └── ...
│   ├── RTXOn.cs
│   └── RTXOn.csproj
│   
├── RTXLib
│    ...
│       
├── RTXLib.Tests
│    ...
```

`render` is an option passed to the main program, which tells it to render a 3D scene parsing a text file. What text file? `pyramid.txt`, located in `examples`.

The option `luminosity` overrides the average luminosity of the image, making the image *brighter* (*darker*) when the luminosity passed is *smaller* (*bigger*) than the original value calculated directly with the pixel values.

This is the basic syntax, for a more in-depth explanation of the process of image generation, take a look at the Professor's [slides].

For a complete list of the options accepted by `RTXOn`, write the following command:

```
$ ./path-to-exe/RTXOn --help
```

That's it, have fun !

## Contributing

In case this project turns out to be useful to someone other than us, [we](#contact)'d be (first of all surprised, but also) very happy to know more about it !

If you have a suggestion that would make the project better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".

Any contribution you make is **greatly appreciated**.

## License

`RTXOn` is distributed under the Apache 2.0 License. See [LICENSE.txt][license] for more information.

<h2 id="contact">Contact the Developers</h2>

Andrea Mastropasqua: [andrea.mastropasqua@studenti.unimi.it](mailto:andrea.mastropasqua@studenti.unimi.it)

Giacomo Riccardo Hvaring: [giacomoriccardo.hvaring@studenti.unimi.it](mailto:giacomoriccardo.hvaring@studenti.unimi.it)

## Acknowledgments

We thank [SixLabors] for releasing [ImageSharp] free of charge: it took care of the conversion to PNG for us !

We thank also all the contributors to [CommandLineParser], for allowing us to integrate seamlessly command line options in our code.

And of course, we thank our parents for sponsoring us while we learned how to make CG balls for 6 months.

[license]: https://github.com/grhvaring/RTXOn/tree/master/LICENSE.txt
[project-link]: https://github.com/grhvaring/RTXOn
[raytracing_course]: https://ziotom78.github.io/raytracing_course
[pytracer]: https://github.com/ziotom78/pytracer
[examples]: https://github.com/grhvaring/RTXOn/tree/master/examples
[slides]: https://ziotom78.github.io/raytracing_course
[sixlabors]: https://sixlabors.com
[imagesharp]: https://sixlabors.com/products/imagesharp/
[commandlineparser]: https://github.com/commandlineparser