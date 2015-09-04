using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Twerkopter;
using Windows.Storage;

namespace Sway_Chopter.Source.Mechanics
{
    public class Score
    {
        Viewport viewport;
        public SpriteFont spriteFont;
        public int score;
        public int highScore;
        public bool display = true;

        public Score(Viewport vp, ContentManager content)
        {
            viewport = vp;
            spriteFont = content.Load<SpriteFont>("ScoreFont");
            score = 0;
            highScore = getHighScore();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //current score

            Vector2 size = spriteFont.MeasureString(score.ToString());

            if (display)
            {
                #region outline
                spriteBatch.DrawString(spriteFont, score.ToString(), new Vector2(MainGame.me.viewport.Width * .5f + 3, MainGame.me.viewport.Height * .3f), Color.Black, 0, size * .5f, 2.5f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, score.ToString(), new Vector2(MainGame.me.viewport.Width * .5f - 3, MainGame.me.viewport.Height * .3f), Color.Black, 0, size * .5f, 2.5f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, score.ToString(), new Vector2(MainGame.me.viewport.Width * .5f, MainGame.me.viewport.Height * .3f + 3), Color.Black, 0, size * .5f, 2.5f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, score.ToString(), new Vector2(MainGame.me.viewport.Width * .5f, MainGame.me.viewport.Height * .3f - 3), Color.Black, 0, size * .5f, 2.5f, SpriteEffects.None, 0f);
                #endregion
                spriteBatch.DrawString(spriteFont, score.ToString(), new Vector2(MainGame.me.viewport.Width * .5f, MainGame.me.viewport.Height * .3f), Color.White, 0, size * .5f, 2.5f, SpriteEffects.None, 0f); 
            }               
        }

        private async void callHS()
        {
            try
            {
                StorageFile sampleFile = await ApplicationData.Current.LocalFolder.GetFileAsync("dataFile.txt");
                highScore = Convert.ToInt32(await FileIO.ReadTextAsync(sampleFile));
            }

            catch (Exception)
            {
            }
        }

        public int getHighScore()
        {
            callHS();
            return highScore;
        }

        public async void saveScore()
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            try { StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt"); } //Create if doesn't exist
            catch { }

            int hs = getHighScore();

            if (score > hs)
            {
                StorageFile sampleFile = await localFolder.GetFileAsync("dataFile.txt");
                await FileIO.WriteTextAsync(sampleFile, score.ToString());
            }
        }
    }
}