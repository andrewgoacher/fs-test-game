module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

type State=
    | Empty
    | TestState of TestState.TestState

let createTestState (bounds:Rectangle) (contentManager:ContentManager)=
    let windowWidth = bounds.Width
    let windowHeight = bounds.Height
    let config:Settings.Config = {width=windowWidth;height=windowHeight}

    let backgroundTexture = Texture.createTexture "bg1" contentManager

    let animationTexture = contentManager.Load<Texture2D>("scottpilgrim_multiple")
    let atlasSize = (108,140)
    let atlas = Texture.createAtlas animationTexture atlasSize

    let charY = windowHeight - 108
    let charX = 75
    let characterPos = new Vector2( (float32 charX), (float32 charY))

    let runRightAnimation = Animation.createAnimationFromAtlas "run-right" atlas [0..7] 150.0 true
    let animations = (Animation.addAnimation {animations=[];currentAnimation=""} runRightAnimation)

    let animationState = Animation.createAnimationState animations characterPos 0.0 true 0

    TestState.createTestState backgroundTexture animationState config        

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
    
