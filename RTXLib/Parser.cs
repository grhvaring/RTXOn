using Xunit;

namespace RTXLib;

public static class Parser
{
    /// <summary>
    /// Read a token from `input_file` and check that it matches `symbol`.
    /// </summary>
    public static void ExpectSymbol(InputStream inputFile, char symbol)
    {
        var token = inputFile.ReadToken();
        var message = $"Got {token} when expecting {symbol}";
        var e = new GrammarError(token.Location, message);
        if (token is not SymbolToken) throw e;
        var symToken = (SymbolToken) token;
        if (symToken.Symbol != symbol) throw e;
    }

    /// <summary>
    /// Read a token from `input_file` and check if it is contained in `keywords`.
    /// </summary>
    /// <returns>Keyword as KeywordEnum object</returns>
    public static KeywordEnum ExpectKeywords(InputStream inputFile, KeywordEnum[] keywords)
    {
        var token = inputFile.ReadToken();

        if (token is not KeywordToken)
            throw new GrammarError(token.Location, $"Keyword expected, got a '{token}' instead.");

        var keywordToken = (KeywordToken) token;

        if (!keywords.Contains(keywordToken.Keyword))
            throw new GrammarError(token.Location, $"Expected a keyword in the list {string.Join(", ", keywords)}");

        return keywordToken.Keyword;
    }

    /// <summary>
    /// Read a token from `input_file` and check if it is a string.
    /// </summary>
    /// <returns>String</returns>
    public static string ExpectString(InputStream inputFile)
    {
        var token = inputFile.ReadToken();

        if (token is not LiteralStringToken)
            throw new GrammarError(token.Location, $"Expected a string, got a '{token}' instead.");

        var literalStringToken = (LiteralStringToken) token;

        return literalStringToken.String;
    }

    /// <summary>
    /// Read a token from `input_file` and check if it is an identifier.
    /// </summary>
    /// <returns>Indentifier as a string</returns>
    public static string ExpectIdentifier(InputStream inputFile)
    {
        var token = inputFile.ReadToken();

        if (token is not IdentifierToken)
            throw new GrammarError(token.Location, $"Expected an identifier, got a '{token}' instead.");

        var identifierToken = (IdentifierToken)token;

        return identifierToken.Identifier;
    }

    public static float ExpectNumber(InputStream stream, Scene scene)
    {
        var token = stream.ReadToken();
        if (token is LiteralNumberToken numberToken) return numberToken.Value;
        if (token is IdentifierToken idToken)
        {
            var variableName = idToken.Identifier;
            if (!scene.FloatVariables.ContainsKey(variableName)) 
                throw new GrammarError(idToken.Location, $"Unknown variable {variableName}.");
            return scene.FloatVariables[variableName];
        }

        throw new GrammarError(token.Location, $"Expected a number, got a {token}.");
    }

    public static Color ParseColor(InputStream stream, Scene scene)
    {
        ExpectSymbol(stream, '[');
        var r = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ',');
        var g = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ',');
        var b = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ']');
        return new Color(r, g, b);
    }
    
    public static Vec ParseVector(InputStream stream, Scene scene)
    {
        ExpectSymbol(stream, '[');
        var x = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ',');
        var y = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ',');
        var z = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ']');
        return new Vec(x, y, z);
    }

    public static Pigment ParsePigment(InputStream stream, Scene scene)
    {
        var keywords = new[]{KeywordEnum.Uniform, KeywordEnum.Checkered, KeywordEnum.Image};
        var location = stream.Location.ShallowCopy();
        var keyword = ExpectKeywords(stream, keywords);
        Pigment result = new UniformPigment();
        switch (keyword)
        {
            case KeywordEnum.Uniform:
            {
                var color = ParseColor(stream, scene);
                result = new UniformPigment(color);
                break;
            }
            case KeywordEnum.Checkered:
            {
                var color1 = ParseColor(stream, scene);
                ExpectSymbol(stream, ',');
                var color2 = ParseColor(stream, scene);
                ExpectSymbol(stream, ',');
                var steps = (int)ExpectNumber(stream, scene);
                result = new CheckeredPigment(color1, color2, steps);
                break;
            }
            case KeywordEnum.Image:
            {
                var fileName = ExpectString(stream);
                result = new ImagePigment(fileName);
                break;
            }
            default:
                Assert.True(false, "This line should be unreachable");
                break;
        }
        return result;
    }

    public static BRDF ParseBRDF(InputStream stream, Scene scene)
    {
        KeywordEnum [] BRDFKEYWORDS = {KeywordEnum.Diffuse, KeywordEnum.Uniform};

        var brdfKeyword = ExpectKeywords(stream, BRDFKEYWORDS);
        ExpectSymbol(stream, '(');
        var pigment = ParsePigment(stream, scene);
        ExpectSymbol(stream, '(');

        if (brdfKeyword == KeywordEnum.Diffuse)
            return new DiffuseBRDF(pigment);

        if (brdfKeyword == KeywordEnum.Uniform)
            return new SpecularBRDF(pigment);

        // NOTE: to be changed and to verify how to handle assert in C#
        throw new Exception("This line should be unreachable");
        
    }

    public static (string, Material) ParseMaterial(InputStream stream, Scene scene)
    {
        var name = ExpectIdentifier(stream);

        ExpectSymbol(stream, '(');
        var brdf = ParseBRDF(stream, scene);
        ExpectSymbol(stream, ',');
        var emittedRadiance = ParsePigment(stream, scene);
        ExpectSymbol(stream, ')');

        var material = new Material(brdf, emittedRadiance);

        return (name, material);
    }

    public static Transformation ParseTransformation(InputStream stream, Scene scene)
    {
        Transformation trasformation = new Transformation();

        KeywordEnum[] TRANSFORMATIONS =
        { KeywordEnum.Identity,
          KeywordEnum.Translation,
          KeywordEnum.RotationX,
          KeywordEnum.RotationY,
          KeywordEnum.RotationZ,
          KeywordEnum.Scaling
        };

        while (true)
        {
            var trasformationKeyword = ExpectKeywords(stream, TRANSFORMATIONS);

            if (trasformationKeyword == KeywordEnum.Identity) { }   // Do nothing if the trasformation is identity
            if (trasformationKeyword == KeywordEnum.Translation)
            {
                ExpectSymbol(stream, '(');
                trasformation *= Transformation.Translation(ParseVector(stream, scene));
                ExpectSymbol(stream, ')');
            }
            else if (trasformationKeyword == KeywordEnum.RotationX)
            {
                ExpectSymbol(stream, '(');
                trasformation *= Transformation.RotationX(ExpectNumber(stream, scene));
                ExpectSymbol(stream, ')');
            }
            else if (trasformationKeyword == KeywordEnum.RotationY)
            {
                ExpectSymbol(stream, '(');
                trasformation *= Transformation.RotationY(ExpectNumber(stream, scene));
                ExpectSymbol(stream, ')');
            }
            else if (trasformationKeyword == KeywordEnum.RotationZ)
            {
                ExpectSymbol(stream, '(');
                trasformation *= Transformation.RotationZ(ExpectNumber(stream, scene));
                ExpectSymbol(stream, ')');
            }
            else if (trasformationKeyword == KeywordEnum.Scaling)
            {
                ExpectSymbol(stream, '(');
                var vec = ParseVector(stream, scene);
                trasformation *= Transformation.Scaling(vec.X, vec.Y, vec.Z);
                ExpectSymbol(stream, ')');
            }

            // Check if next token is also a chained trasformation
            var nextKeyword = stream.ReadToken();

            if ((nextKeyword is not SymbolToken) || (((SymbolToken)nextKeyword).Symbol != '*'))
            {
                stream.UnreadToken(nextKeyword);
                break;
            }
        }

        return trasformation;
    }

    public static Sphere ParseSphere(InputStream stream, Scene scene)
    {
        ExpectSymbol(stream, '(');
        var location = stream.Location.ShallowCopy();
        var materialName = ExpectIdentifier(stream);
        if (!scene.Materials.ContainsKey(materialName))
            throw new GrammarError(location, $"Unknown material {materialName}.");
        var material = scene.Materials[materialName];
        ExpectSymbol(stream, ',');
        var transformation = ParseTransformation(stream, scene);
        ExpectSymbol(stream, ')');
        
        return new Sphere(material, transformation);
    }

    public static Plane ParsePlane(InputStream stream, Scene scene)
    {
        ExpectSymbol(stream, ',');
        var location = stream.Location.ShallowCopy();
        var materialName = ExpectIdentifier(stream);
        if (!scene.Materials.ContainsKey(materialName))
            throw new GrammarError(location, $"Unknown material {materialName}.");
        var material = scene.Materials[materialName];
        ExpectSymbol(stream, ',');
        var transformation = ParseTransformation(stream, scene);
        ExpectSymbol(stream, ')');
        
        return new Plane(material, transformation);
    }

    public static ICamera ParseCamera(InputStream stream, Scene scene)
    {
        ExpectSymbol(stream, '(');
        var type = ExpectKeywords(stream, new[] { KeywordEnum.Orthogonal, KeywordEnum.Perspective });
        ExpectSymbol(stream, ',');
        var transformation = ParseTransformation(stream, scene);
        ExpectSymbol(stream, ',');
        var aspectRatio = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ',');
        var distance = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ')');

        return type is KeywordEnum.Orthogonal
            ? new OrthogonalCamera(aspectRatio, transformation)
            : new PerspectiveCamera(distance, aspectRatio, transformation);
}

    /// <summary>
    /// Method <c>ParseScene</c> reads a scene from a stream and returns a <c>Scene</c> object.
    /// </summary>
    /// <param name="stream">The stream from which the file is read.</param>
    /// <param name="variables">The list of the variables.</param>
    /// <returns>A scene with the collection of object (world, camera, materials and float variables) described in the stream.</returns>
    /// <exception cref="GrammarError"></exception>
    public static Scene ParseScene(InputStream stream, Dictionary<string, float> variables)
    {
        Scene scene = new Scene();
        scene.FloatVariables = variables; //Shallow Copy
        scene.OverriddenVariables = new SortedSet<string>(variables.Keys);

        while (true)
        {
            var what = stream.ReadToken();

            if (what is StopToken)
                break;

            if (what is not KeywordToken)
                throw new Exception("This line should be unreachable");

            var whatKeyword = (KeywordToken)what;   //At this point we are sure the object is a KeywordToken

            if (whatKeyword.Keyword == KeywordEnum.Float)
            {
                var variableName = ExpectIdentifier(stream);
                var variableLocation = stream.Location;

                ExpectSymbol(stream, '(');
                var variableValue = ExpectNumber(stream, scene);
                ExpectSymbol(stream, ')');

                if (scene.FloatVariables.ContainsKey(variableName) && !(scene.OverriddenVariables.Contains(variableName)))
                    throw new GrammarError(variableLocation, $"Variable «{variableLocation}» cannot be redefinied.");

                if (!scene.OverriddenVariables.Contains(variableName))
                    scene.FloatVariables.Add(variableName, variableValue);
            }

            else if (whatKeyword.Keyword == KeywordEnum.Sphere)
                scene.World.Add(ParseSphere(stream, scene));

            else if (whatKeyword.Keyword == KeywordEnum.Plane)
                scene.World.Add(ParsePlane(stream, scene));

            else if (whatKeyword.Keyword == KeywordEnum.Camera)
            {
                if (scene.Camera == null)
                    throw new GrammarError(whatKeyword.Location, $"You cannot define more that one camera.");

                scene.Camera = ParseCamera(stream, scene);
            }

            else if (whatKeyword.Keyword == KeywordEnum.Material)
            {
                (var name, var material) = ParseMaterial(stream, scene);
                scene.Materials.Add(name, material);
            }
        }

        return scene;
    }

}