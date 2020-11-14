using System.Linq;
using OpenSage.Content.Translation;
using OpenSage.Data.Apt;
using OpenSage.Data.Apt.Characters;
using OpenSage.Gui.Apt.ActionScript;
using Veldrid;

namespace OpenSage.Gui.Apt
{
    public sealed class RenderItem : DisplayItem
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private TimeInterval _lastUpdate;
        private bool IsHovered { get; set; }

        public delegate void CustomRenderCallback(AptRenderingContext context, Geometry geometry, Texture originalTexture);
        public CustomRenderCallback RenderCallback;

        public override void Create(Character character, AptContext context, SpriteItem parent = null)
        {
            Character = character;
            Context = context;
            Parent = parent;
            ScriptObject = new ObjectContext(this);
            Name = "";
            Visible = true;
            IsHovered = false;
        }

        public override void Update(TimeInterval gt)
        {
            // Currently only Text needs to be updated
            if (!(Character is Text t))
            {
                return;
            }

            if ((gt.TotalTime - _lastUpdate.TotalTime).TotalMilliseconds < Context.MillisecondsPerFrame)
            {
                return;
            }

            _lastUpdate = gt;
            if (t.Value.Length > 0)
            {
                try
            {
                    var val = ScriptObject.ResolveValue(t.Value, ScriptObject);
                    if (val.Type != ValueType.Undefined)
                        t.Content = val.ToString();
                }
                catch (System.Exception e)
                {
                    Logger.Warn($"Failed to resolve text value: {e}");
                }

            }

            // localize our content
            t.LocalizedContent = t.Content
                .Replace("$", "APT:") // All string values begin with $
                .Split('&').First()   // Query strings after ampersand
                .Translate();
        }

        protected override void RenderImpl(AptRenderingContext renderingContext)
        {
            if (!Visible)
                return;

            renderingContext.PushTransform(Transform);

            switch (Character)
            {
                case Shape s:
                    var geometry = Context.GetGeometry(s.Geometry, Character);
                    if (RenderCallback != null)
                    {
                        RenderCallback(renderingContext, geometry, Texture);
                    }
                    else
                    {
                        renderingContext.RenderGeometry(geometry, Texture);
                    }

                    if (Highlight)
                    {
                        renderingContext.RenderOutline(geometry);
                    }
                    break;

                case Text t:
                    renderingContext.RenderText(t);
                    break;
            }

            renderingContext.PopTransform();
        }
    }
}
