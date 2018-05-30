module TestState

open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework
open Animation
open Texture

type TestState = {
    background:Texture.Sprite
    animation: AnimationState
}

let createTestState texture animation (config:Settings.Config)=
    let bounds = new Rectangle(0,0,config.width, config.height)
    let background = Texture.createSprite texture bounds |> Texture.Sprite.StaticSprite
    {background=background;animation=animation}

let draw (spriteBatch:SpriteBatch) (testState:TestState) =
    Texture.draw spriteBatch testState.background None
    
    Animation.draw spriteBatch testState.animation

let update (gameTime:GameTime) (testState:TestState)=
    let newAnimationState = Animation.update gameTime testState.animation
    {background=testState.background;animation=newAnimationState}




