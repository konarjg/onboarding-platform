namespace OnboardingPlatform.Configuration;

using Domain.Commands;
using Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

public class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver {
  public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options) {
    JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);
    
    Type[] commandBaseTypes = [typeof(CreateContentSectionCommand), typeof(UpdateContentSectionCommand)];
    
    if (commandBaseTypes.Contains(jsonTypeInfo.Type)) {
      jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
        TypeDiscriminatorPropertyName = "type",
        IgnoreUnrecognizedTypeDiscriminators = true,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        DerivedTypes = {
          new JsonDerivedType(typeof(CreateMarkdownSectionCommand), "Markdown"),
          new JsonDerivedType(typeof(CreateImageSectionCommand), "Image"),
          new JsonDerivedType(typeof(UpdateMarkdownSectionCommand), "Markdown"),
          new JsonDerivedType(typeof(UpdateImageSectionCommand), "Image")
        }
      };
    }
    
    Type[] dtoBaseTypes = [typeof(CreateContentSectionRequest), typeof(UpdateContentSectionRequest)];
    
    if (dtoBaseTypes.Contains(jsonTypeInfo.Type)) {
      jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
        TypeDiscriminatorPropertyName = "type",
        IgnoreUnrecognizedTypeDiscriminators = true,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        DerivedTypes = {
          new JsonDerivedType(typeof(CreateMarkdownSectionRequest), nameof(ContentSectionTypeDto.Markdown)),
          new JsonDerivedType(typeof(CreateImageSectionRequest), nameof(ContentSectionTypeDto.Image)),
          new JsonDerivedType(typeof(UpdateMarkdownSectionRequest), nameof(ContentSectionTypeDto.Markdown)),
          new JsonDerivedType(typeof(UpdateImageSectionRequest), nameof(ContentSectionTypeDto.Image))
        }
      };
    }

    return jsonTypeInfo;
  }
}