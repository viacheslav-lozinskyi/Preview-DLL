
namespace resource.preview
{
    public class DLL : cartridge.AnyPreview
    {
        protected override void _Execute(atom.Trace context, string url)
        {
            //var a_Context = new XmlDocument();
            //{
            //    a_Context.Load(url);
            //}
            //{
            //    __Execute(context, a_Context.DocumentElement, 1);
            //}
        }

        //private static void __Execute(atom.Trace context, XmlNode node, int level)
        //{
        //    if (node == null)
        //    {
        //        return;
        //    }
        //    if (string.IsNullOrEmpty(node.Name))
        //    {
        //        return;
        //    }
        //    if (GetState() == STATE.EXECUTE)
        //    {
        //        if (string.IsNullOrEmpty(node.Name) == false)
        //        {
        //            if ((node.NodeType != XmlNodeType.Comment) && __IsContentFound(node))
        //            {
        //                context.
        //                    Clear().
        //                    SetContent(node.Name).
        //                    SetValue(__GetValue(node)).
        //                    SetComment(__GetComment(node)).
        //                    SetPattern(__GetPattern(node)).
        //                    SetFlag((level == 1) ? atom.Trace.NAME.FLAG.EXPAND : "").
        //                    SetHint("Tag type").
        //                    SetLevel(level).
        //                    Send();
        //            }
        //        }
        //        if ((node.Attributes != null) && (node.NodeType == XmlNodeType.Element))
        //        {
        //            foreach (XmlAttribute a_Context in node.Attributes)
        //            {
        //                if (GetState() != STATE.EXECUTE)
        //                {
        //                    return;
        //                }
        //                {
        //                    __Execute(context, a_Context, level + 1);
        //                }
        //            }
        //        }
        //        if ((node.ChildNodes != null) && (node.NodeType == XmlNodeType.Element))
        //        {
        //            foreach (XmlNode a_Context in node.ChildNodes)
        //            {
        //                if (GetState() != STATE.EXECUTE)
        //                {
        //                    return;
        //                }
        //                {
        //                    __Execute(context, a_Context, level + 1);
        //                }
        //            }
        //        }
        //    }
        //}

        //private static bool __IsContentFound(XmlNode node)
        //{
        //    if (node.Name == "#text")
        //    {
        //        var a_Context = node.ParentNode;
        //        if ((a_Context.Attributes != null) && (a_Context.Attributes.Count > 0))
        //        {
        //            return true;
        //        }
        //        if ((a_Context.ChildNodes != null) && (a_Context.ChildNodes.Count == 1))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //private static bool __IsChildrenFound(XmlNode node)
        //{
        //    if ((node.Attributes != null) && (node.Attributes.Count > 0))
        //    {
        //        return true;
        //    }
        //    if (node.ChildNodes != null)
        //    {
        //        if (node.ChildNodes.Count != 1)
        //        {
        //            return true;
        //        }
        //        if (node.ChildNodes[0].Name == "#text")
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //private static string __GetComment(XmlNode node)
        //{
        //    switch (node.NodeType)
        //    {
        //        case XmlNodeType.None: return "None";
        //        case XmlNodeType.Element: return "Element";
        //        case XmlNodeType.Attribute: return "Attribute";
        //        case XmlNodeType.Text: return "Text";
        //        case XmlNodeType.CDATA: return "CDATA";
        //        case XmlNodeType.EntityReference: return "Entity Reference";
        //        case XmlNodeType.Entity: return "Entity";
        //        case XmlNodeType.ProcessingInstruction: return "Processing Instruction";
        //        case XmlNodeType.Comment: return "Comment";
        //        case XmlNodeType.Document: return "Document";
        //        case XmlNodeType.DocumentType: return "Document Type";
        //        case XmlNodeType.DocumentFragment: return "Document Fragment";
        //        case XmlNodeType.Notation: return "Notation";
        //        case XmlNodeType.Whitespace: return "Whitespace";
        //        case XmlNodeType.SignificantWhitespace: return "Significant Whitespace";
        //        case XmlNodeType.EndElement: return "End Element";
        //        case XmlNodeType.EndEntity: return "End Entity";
        //        case XmlNodeType.XmlDeclaration: return "Declaration";
        //    }
        //    return "";
        //}

        //private static string __GetValue(XmlNode node)
        //{
        //    if (string.IsNullOrWhiteSpace(node.Value))
        //    {
        //        return __IsChildrenFound(node) ? "" : GetCleanString(node.InnerText);
        //    }
        //    else
        //    {
        //        return GetCleanString(node.Value);
        //    }
        //}

        //private static string __GetPattern(XmlNode node)
        //{
        //    if (node.NodeType == XmlNodeType.Attribute)
        //    {
        //        return atom.Trace.NAME.PATTERN.PARAMETER;
        //    }
        //    if ((node.Attributes != null) && (node.Attributes.Count > 0))
        //    {
        //        return "";
        //    }
        //    if ((node.ChildNodes != null) && (node.ChildNodes.Count > 0))
        //    {
        //        return __IsChildrenFound(node) ? "" : atom.Trace.NAME.PATTERN.VARIABLE;
        //    }
        //    return atom.Trace.NAME.PATTERN.VARIABLE;
        //}
    };
}
