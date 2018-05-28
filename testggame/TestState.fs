module TestState

open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework

type TestState = {
    texture:Texture2D
    bounds:Rectangle
    animation: AnimatedSprite.Animation
}

let createTestState texture animation (config:Settings.Config)=
    let bounds = new Rectangle(0,0,config.width, config.height)
    {texture=texture;bounds=bounds;animation=animation}

let draw (spriteBatch:SpriteBatch) (testState:TestState) =
    spriteBatch.Draw(
        testState.texture,
        testState.bounds,
        Color.White)
    
    AnimatedSprite.draw spriteBatch testState.animation

let update (gameTime:GameTime) (testState:TestState)=
    let newAnimationState = AnimatedSprite.update gameTime testState.animation
    let bounds = testState.bounds
    let config: Settings.Config  = {width=bounds.Width; height = bounds.Height}
    createTestState testState.texture newAnimationState config




