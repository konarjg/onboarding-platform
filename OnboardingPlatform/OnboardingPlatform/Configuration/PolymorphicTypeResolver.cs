namespace OnboardingPlatform.Configuration;

using Domain.Commands;
using Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

public class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver {
  public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options) {
    JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);
    
    // 1. Handle Commands (Internal Domain)
    if (jsonTypeInfo.Type == typeof(CreateContentSectionCommand)) {
      jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
        TypeDiscriminatorPropertyName = "type",
        IgnoreUnrecognizedTypeDiscriminators = true,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        DerivedTypes = {
          new JsonDerivedType(typeof(CreateMarkdownSectionCommand), "Markdown"),
          new JsonDerivedType(typeof(CreateImageSectionCommand), "Image")
        }
      };
    }
    else if (jsonTypeInfo.Type == typeof(UpdateContentSectionCommand)) {
      jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
        TypeDiscriminatorPropertyName = "type",
        IgnoreUnrecognizedTypeDiscriminators = true,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        DerivedTypes = {
          new JsonDerivedType(typeof(UpdateMarkdownSectionCommand), "Markdown"),
          new JsonDerivedType(typeof(UpdateImageSectionCommand), "Image")
        }
      };
    }
    
    // 2. Handle Requests (Input DTOs)
    else if (jsonTypeInfo.Type == typeof(CreateContentSectionRequest)) {
      jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
        TypeDiscriminatorPropertyName = "type",
        IgnoreUnrecognizedTypeDiscriminators = true,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        DerivedTypes = {
          new JsonDerivedType(typeof(CreateMarkdownSectionRequest), nameof(ContentSectionTypeDto.Markdown)),
          new JsonDerivedType(typeof(CreateImageSectionRequest), nameof(ContentSectionTypeDto.Image))
        }
      };
    }
    else if (jsonTypeInfo.Type == typeof(UpdateContentSectionRequest)) {
      jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
        TypeDiscriminatorPropertyName = "type",
        IgnoreUnrecognizedTypeDiscriminators = true,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        DerivedTypes = {
          new JsonDerivedType(typeof(UpdateMarkdownSectionRequest), nameof(ContentSectionTypeDto.Markdown)),
          new JsonDerivedType(typeof(UpdateImageSectionRequest), nameof(ContentSectionTypeDto.Image))
        }
      };
    }

    // 3. Handle Responses (Output DTOs) <--- THIS WAS MISSING
    else if (jsonTypeInfo.Type == typeof(ContentSectionResponse)) {
      jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
        TypeDiscriminatorPropertyName = "type", // Matches the existing 'Type' property
        IgnoreUnrecognizedTypeDiscriminators = true,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        DerivedTypes = {
          new JsonDerivedType(typeof(MarkdownSectionResponse), nameof(ContentSectionTypeDto.Markdown)),
          new JsonDerivedType(typeof(ImageSectionResponse), nameof(ContentSectionTypeDto.Image))
        }
      };
    }

    return jsonTypeInfo;
  }
}