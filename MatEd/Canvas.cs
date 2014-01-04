using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MatEd
{
    class Canvas
    {
        private List<EffectParameter> textures;
        private Effect effect;
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background.GetTexture(), Vector2.Zero, Color.White);
            spriteBatch.End();
        }
        public void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            Texture2D checkers = Content.Load<Texture2D>("checkers");
            background = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0, SurfaceFormat.Color);
            RenderTarget2D tmp = (RenderTarget2D)background.GraphicsDevice.GetRenderTarget(0);
            background.GraphicsDevice.SetRenderTarget(0, background);
            spriteBatch.Begin();
            for (int i = 0; i <= background.Width / checkers.Width; ++i )
            {
                for (int j = 0; j <= background.Height / checkers.Height; ++j)
                {
                    spriteBatch.Draw(checkers, new Vector2(i * checkers.Width, j * checkers.Height), Color.White);
                }
            }
            spriteBatch.End();
            background.GraphicsDevice.SetRenderTarget(0, tmp);
            
        }
        RenderTarget2D background;
    }
}
