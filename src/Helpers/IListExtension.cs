using System.Collections;

public static class IListExtension
{
  public static void AddRange(this IList list1, IList list2)
  {
    if (list2 == null) return;
    if (list2.Count == 0) return;
    Type targetType = list1.GetType().GetGenericArguments().FirstOrDefault();
    Type sourceType = list2[0].GetType();
    if (targetType != null && targetType != sourceType)
      throw new InvalidOperationException($"Incompatibilidade de tipos: {sourceType.Name} não pode ser adicionado a {targetType.Name}");
    foreach (var item in list2)
      {
        list1.Add(item);
      }
  }
}
