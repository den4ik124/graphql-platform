namespace HotChocolate.Fusion.Planning.Collections;

public sealed class CompositeInputFieldCollection(
    IEnumerable<CompositeInputField> fields)
    : CompositeFieldCollection<CompositeInputField>(fields);
