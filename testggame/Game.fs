module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

type State=
    | Empty
    | TestState of TestState.TestState

type Settings = {
    width:int
    height:int
}

let createTexture name (content:ContentManager)=
    content.Load<Texture2D>(name)

let createTestState (bounds:Rectangle) (contentManager:ContentManager)=
    let windowWidth = bounds.Width
    let windowHeight = bounds.Height
    let config:Settings.Config = {width=windowWidth;height=windowHeight}

    let animationTexture = contentManager.Load<Texture2D>("scottpilgrim_multiple")
    let atlasSize = new Point(108,140)
    let atlas = AnimatedSprite.createAtlas animationTexture atlasSize

    let charY = windowHeight - 108
    let charX = 75
    let characterPos = new Vector2( (float32 charX), (float32 charY))

    let animation = AnimatedSprite.createAnimation atlas 0 0.0 180.0 true characterPos false
    TestState.createTestState (createTexture "bg1" contentManager) animation config        

type Game() as this=
    inherit Microsoft.Xna.Framework.Game()
    
    let _ = new GraphicsDeviceManager(this)
    let mutable state = State.Empty
    let mutable spritebatch: SpriteBatch = null

    do this.Content.RootDirectory <- "Content"

    override this.Initialize()=
        spritebatch <- new SpriteBatch(this.GraphicsDevice)
        ()

    override this.Draw(gameTime:GameTime)=
        base.Draw(gameTime)
        this.GraphicsDevice.Clear(Color.CornflowerBlue)

        spritebatch.Begin()

        match state with 
        | Empty -> ()
        | TestState ts -> 
            TestState.draw spritebatch ts
        ()

        spritebatch.End()
    
    override this.Update(gameTime:GameTime)=
        match state with 
        | Empty -> 
            let newState = createTestState this.Window.ClientBounds this.Content
            state <- State.TestState newState
        | TestState ts ->
            let newState = (TestState.update gameTime ts)
            state <- State.TestState newState
        base.Update(gameTime)
    
