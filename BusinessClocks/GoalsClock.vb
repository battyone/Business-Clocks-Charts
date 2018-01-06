﻿Imports System.Drawing
Imports System.Windows.Forms

Namespace ExecutiveClocks


    Public Class GoalsClock
        Implements IClock, IDisposable
        Private Const VersionString As String = "5.0.0.0"
        Public Shared Property Version As New Version(VersionString)

        Private Const INNER_RECT_RATIO As Single = 0.5
        Private _ClockWidth As Int16
        ''' <summary>
        ''' Gets or sets the clock width
        ''' </summary>
        ''' <returns></returns>
        Public Property ClockWidth As Int16
            Get
                Return _ClockWidth
            End Get
            Private Set(value As Int16)
                _ClockWidth = value
            End Set
        End Property
        Private _ClockHeight As Int16
        ''' <summary>
        ''' Gets or sets the clock height
        ''' </summary>
        ''' <returns></returns>
        Public Property ClockHeight As Int16
            Get
                Return _ClockHeight
            End Get
            Private Set(value As Int16)
                _ClockHeight = value
            End Set
        End Property
        Private _xPos As Int16
        ''' <summary>
        ''' Gets the x position of the rectangle that containing the clock
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property xPos As Int16
            Get
                If OuterCircleWeight = 0 Or OuterCircleWeight = Nothing Then Throw New Exception("OuterSquareWidth Must Have Value")
                Return OuterCircleWeight / 2
            End Get
        End Property
        Private _yPos As Int16
        ''' <summary>
        ''' Gets the y position of the rectangle that containing the clock
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property yPos As Int16
            Get
                If OuterCircleWeight = 0 Or OuterCircleWeight = Nothing Then Throw New Exception("OuterSquareWidth Must Have Value")
                Return OuterCircleWeight / 2
            End Get
        End Property
        Public _Clock As PictureBox
        Public Property Clock As PictureBox
            Get
                If _Clock Is Nothing Then
                    _Clock = New PictureBox
                    Return _Clock
                Else
                    Return _Clock
                End If
            End Get
            Set(value As PictureBox)
                _Clock = value
            End Set
        End Property
        Private _panelColor As Color
        ''' <summary>
        ''' Gets or sets the clock background color
        ''' </summary>
        ''' <returns></returns>
        Public Property ClockBackGroundColor As Color
            Get
                Return _panelColor
            End Get
            Set(value As Color)
                _panelColor = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Private PercentTextColor As Color
        ' rectangles to store squares
        Protected OuterRect As Rectangle
        Protected InnerRect As Rectangle
        Protected InnerStringBrush As Brush
        Protected _InnerStringColor As Color
        ''' <summary>
        ''' Gets or sets the font color of the text thats describes the PercentOfGoals percentage
        ''' </summary>
        ''' <returns></returns>
        Public Property FontColor As Color
            Get
                Return _InnerStringColor
            End Get
            Set(value As Color)
                _InnerStringColor = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Private _InnerStringFontSize As Byte
        Private Property InnerStringFontSize As Byte
            Get
                Return _InnerStringFontSize
            End Get
            Set(ByVal value As Byte)
                If value = 0 Then
                    _InnerStringFontSize = 6 ' minimum readable font size
                ElseIf value > 48 Then
                    _InnerStringFontSize = 48
                Else
                    _InnerStringFontSize = value
                End If
            End Set
        End Property
        Private _ClockFont As Font
        ''' <summary>
        ''' Gets or sets the font of the text that describes the PercentOfGoals percentage
        ''' </summary>
        ''' <returns></returns>
        Public Property ClockFont As Font
            Get
                If _ClockFont Is Nothing Then
                    _ClockFont = LoadFont()
                    Return _ClockFont
                Else
                    Return _ClockFont
                End If
            End Get
            Set(value As Font)
                _ClockFont = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        ' inner Circle
        Protected InnerCirclePen As Pen
        Protected _InnerCirclePen_Color As Color
        ''' <summary>
        ''' Gets or sets the color of the inner arc
        ''' </summary>
        ''' <returns></returns>
        Public Property InnerCircleColor As Color
            Get
                Return _InnerCirclePen_Color
            End Get
            Set(value As Color)
                _InnerCirclePen_Color = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Private _InnerCircleWeight As Byte
        ''' <summary>
        ''' Gets or sets the pixel weight (wideness) of the inner arc
        ''' </summary>
        ''' <returns></returns>
        Public Property InnerCircleWeight As Byte
            Get
                Return _InnerCircleWeight
            End Get
            Set(value As Byte)
                _InnerCircleWeight = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        ' outer square
        Protected OuterCirclePen As Pen
        Protected _OuterCirclePen_Color As Color
        ''' <summary>
        ''' Gets or sets the color of the outer arc
        ''' </summary>
        ''' <returns></returns>
        Public Property OuterCircleColor As Color
            Get
                Return _OuterCirclePen_Color
            End Get
            Set(value As Color)
                _OuterCirclePen_Color = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Protected _OuterCirclePen_Weight As Byte
        ''' <summary>
        ''' Gets or sets the pixel weight (wideness) of the outer arc
        ''' </summary>
        ''' <returns></returns>
        Public Property OuterCircleWeight As Byte
            Get
                Return _OuterCirclePen_Weight
            End Get
            Set(value As Byte)
                _OuterCirclePen_Weight = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Private _PercentOfGoals As Single ' to calculate the goals deg arc
        ''' <summary>
        ''' Gets or sets the the clock value. for example value of 0.1F = 10%, value of 0.5F = 50%,value of 1.1F = 110% etc...
        ''' </summary>
        ''' <returns></returns>
        Public Property PercentOfGoals As Single
            Get
                Return _PercentOfGoals ' * 100
            End Get
            Set(ByVal value As Single)
                _PercentOfGoals = value
            End Set
        End Property
        ''' <summary>
        ''' Gets the the clock deciaml value
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GoalsPercentDecimalValue As Single
            Get
                Return _PercentOfGoals
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets a value indicating whether animation will be activated or not
        ''' </summary>
        Public Animate As Boolean
        Protected _AnimationLength As IClock.AnimationLength = IClock.AnimationLength.SuperFast ' default
        ''' <summary>
        ''' Gets or sets animation speed (milisec) based on AnimationLength Enumeration: SuperFast = 1, Fast = 4, ModerateFast = 8, Moderate = 15, ModerateSlow = 28, Slow = 50, SuperSlow = 80
        ''' </summary>
        ''' <returns></returns>
        Public Property AnimationLength As IClock.AnimationLength
            Get
                Return _AnimationLength
            End Get
            Set(value As IClock.AnimationLength)
                _AnimationLength = value
                ' it can be set the timer interval from enum and also from TimerInterval property
                TimerInterval = value
            End Set
        End Property
        Protected _TimerAnimation As Timer
        Protected Overridable Property TimerAnimatiom As Timer
            Get
                If IsNothing(_TimerAnimation) Then
                    _TimerAnimation = New Timer
                    Return _TimerAnimation
                Else
                    Return _TimerAnimation
                End If
            End Get
            Set(value As Timer)
                _TimerAnimation = value
            End Set
        End Property
        Protected _TimerInterval As Integer = 28
        ''' <summary>
        ''' Gets or sets the ticks interval for the internal timer that is handaling the animation. recomended value is 4 (default).
        ''' </summary>
        ''' <returns></returns>
        Public Property TimerInterval As Integer
            Get
                Return _TimerInterval
            End Get
            Set(value As Integer)
                _TimerInterval = value
            End Set
        End Property

        Protected ReadOnly Property OuterRectangleWidth As Int16
            Get
                Return ClockWidth - OuterCircleWeight - 2
            End Get
        End Property
        Protected ReadOnly Property OuterRectangleHeight As Int16
            Get
                Return ClockHeight - OuterCircleWeight - 2
            End Get
        End Property

#Region "Constructors"
        Sub New(ByVal clockWidth As Int16, ByVal clockHeight As Int16, ByVal GoalsPercent As Single)
            Me.ClockWidth = clockWidth
            Me.ClockHeight = clockHeight
            PercentOfGoals = GoalsPercent
            DefaultValues()
        End Sub
        Protected Overridable Sub DefaultValues()
            FontColor = Color.White
            OuterCircleColor = GUI.Blue
            InnerCircleColor = Color.Yellow
            ClockBackGroundColor = Color.FromArgb(20, 20, 20)
            InnerStringFontSize = 12
            OuterCircleWeight = 15
            InnerCircleWeight = 9
            ' recomended animation interval
            TimerInterval = 4
        End Sub
        Sub New(ByVal clockWidth As Int16, ByVal clockHeight As Int16,
                ByVal GoalsPercent As Single, ByVal OuterCircleWeight As Byte)
            Me.New(clockWidth, clockHeight, GoalsPercent)
            ' overrides given value in DefaultValues() 
            Me.OuterCircleWeight = OuterCircleWeight
        End Sub

        Sub New(ByVal clockWidth As Int16, ByVal clockHeight As Int16,
               ByVal GoalsPercent As Single, ByVal OuterCircleWeight As Byte, ByVal InnerCircleWeight As Byte)
            Me.New(clockWidth, clockHeight, GoalsPercent)
            ' overrides given value in DefaultValues() 
            Me.OuterCircleWeight = OuterCircleWeight
            Me.InnerCircleWeight = InnerCircleWeight
        End Sub

        Sub New(ByVal clockWidth As Int16, ByVal clockHeight As Int16,
            ByVal GoalsPercent As Single, ByVal OuterCircleWeight As Byte, ByVal InnerCircleWeight As Byte,
            ByVal OuterCircleColor As Color)
            Me.New(clockWidth, clockHeight, GoalsPercent)
            ' overrides given values in DefaultValues() 
            Me.OuterCircleWeight = OuterCircleWeight
            Me.InnerCircleWeight = InnerCircleWeight
            Me.OuterCircleColor = OuterCircleColor
        End Sub

        Sub New(ByVal clockWidth As Int16, ByVal clockHeight As Int16,
        ByVal GoalsPercent As Single, ByVal OuterCircleWeight As Byte, ByVal InnerCircleWeight As Byte,
        ByVal OuterCircleColor As Color, InnerCircleColor As Color)
            Me.New(clockWidth, clockHeight, GoalsPercent)
            ' overrides given values in DefaultValues()
            Me.OuterCircleWeight = OuterCircleWeight
            Me.InnerCircleWeight = InnerCircleWeight
            Me.OuterCircleColor = OuterCircleColor
            Me.InnerCircleColor = InnerCircleColor
        End Sub

        Sub New(ByVal clockWidth As Int16, ByVal clockHeight As Int16,
      ByVal GoalsPercent As Single, ByVal OuterCircleWeight As Byte, ByVal InnerCircleWeight As Byte,
      ByVal OuterCircleColor As Color, InnerCircleColor As Color, ByVal PanelColor As Color)
            Me.New(clockWidth, clockHeight, GoalsPercent)
            ' overrides given values in DefaultValues()
            Me.OuterCircleWeight = OuterCircleWeight
            Me.InnerCircleWeight = InnerCircleWeight
            Me.OuterCircleColor = OuterCircleColor
            Me.InnerCircleColor = InnerCircleColor
            Me.ClockBackGroundColor = PanelColor
        End Sub
#End Region

        ''' <summary>
        ''' 
        '''  Initilizing the clock object, must be called after all properties are setted
        ''' </summary>
        ''' <remarks></remarks>
        Public Overridable Sub Create(ByVal AnimateClock As Boolean)
            If AnimateClock = True Then
                Me.Animate = True
            Else
                Me.Animate = False
            End If
            ' panel
            Clock.BackColor = ClockBackGroundColor
            Clock.Size = New Size(ClockWidth, ClockHeight)
            ' subscribe to the panel's paint event
            AddHandler Clock.Paint, AddressOf clockPanel_Paint
        End Sub

        Private counter As Int16 = 0
        Private sweepAngle As Int16 = 0
        Protected Overridable Sub clockPanel_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)

            Try
                Dim Gclock As Graphics = e.Graphics 'Local variable only, as the Graphics object might change.
                ' create outer rectangle
                OuterRect = New Rectangle(xPos, yPos, ClockWidth - OuterCircleWeight - 2, ClockHeight - OuterCircleWeight - 2)
                ' create inner rectangle
                Dim w, h, x, y As Integer
                getInnerRectSizeAndLocation(w, h, x, y)
                InnerRect = New Rectangle(x, y, w, h)
                ' create outer circle
                OuterCirclePen = New Pen(OuterCircleColor, OuterCircleWeight)
                Gclock.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                Gclock.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit
                Gclock.DrawArc(OuterCirclePen, OuterRect, 1.0F, 360.0F)
                ' create inner circle
                InnerCirclePen = New Pen(InnerCircleColor, InnerCircleWeight)
                sweepAngle = getSweepAngleFromGoalsPercent()

                If Animate = True Then
                    If IsNothing(_TimerAnimation) Then
                        TimerAnimatiom.Interval = TimerInterval
                        AddHandler TimerAnimatiom.Tick, AddressOf TimerAnimatiom_Tick
                        TimerAnimatiom.Start()
                    End If
                    Gclock.DrawArc(InnerCirclePen, OuterRect, -90.5F, counter)
                Else
                    Gclock.DrawArc(InnerCirclePen, OuterRect, -90.5F, sweepAngle)
                End If

                If ClockFont IsNot Nothing Then ' if nothing LoadFont() determined that clock is to small for text inside
                    ' draw goals string inside inner rect
                    InnerStringBrush = New SolidBrush(FontColor)
                    ' Create a StringFormat object with the each line of text, and the block
                    ' of text centered on the page.
                    Dim stringFormat As New StringFormat()
                    stringFormat.Alignment = StringAlignment.Center
                    stringFormat.LineAlignment = StringAlignment.Center

                    Gclock.DrawString(getPercentString(), ClockFont, InnerStringBrush, InnerRect, stringFormat)
                End If

            Catch ex As Exception
                Debug.WriteLine("ERROR: " & ex.Message.ToString)
            End Try

        End Sub

        Protected Overridable Sub TimerAnimatiom_Tick(sender As Object, e As EventArgs)

            If counter >= sweepAngle Then
                TimerAnimatiom.Stop()
                counter = 0
                TimerAnimatiom.Dispose()
                TimerAnimatiom = Nothing
                Animate = False ' when GUI is updating dont show animation unless there is excluded  RefreshClock(True)
                counter = sweepAngle ' for fixing with the last refres call if counter > sweepangle
                Clock.Refresh() ' last refresh for fixing over rated counter value
                Exit Sub
            Else
                counter += 20
            End If


            ' refresh clock without animation for unpixelized shape
            Clock.Refresh()

        End Sub


        Protected Sub getInnerRectSizeAndLocation(ByRef w As Integer, ByRef h As Integer, ByRef x As Integer, ByRef y As Integer)
            w = OuterRectangleWidth * INNER_RECT_RATIO
            h = OuterRectangleHeight * INNER_RECT_RATIO
            x = (OuterRectangleWidth / 2) - (w / 2) + (OuterCircleWeight / 2) + 1 ' add 1 because panel size is clockHeight - OuterSquareWidth - 2
            y = (OuterRectangleHeight / 2) - (h / 2) + (OuterCircleWeight / 2) + 1
        End Sub

        Protected Overridable Function getPercentString() As String
            Return FormatPercent(GoalsPercentDecimalValue, 0)
        End Function

        Protected Function getSweepAngleFromGoalsPercent() As Single
            Dim result As Single = 360 * GoalsPercentDecimalValue
            If result > 360 Then
                result = 360
            ElseIf result < 0 Then
                result = 0
            End If
            Return result
        End Function

        ''' <summary>
        ''' Refreshes and redraw the graphics of the clock object, call this method after you change properties (PercentOfGoals, InnerCircleColor for example)
        ''' </summary>
        ''' <param name="AnimateClock">indicates if animation is requeried</param>
        Public Overridable Sub RefreshClock(ByVal AnimateClock As Boolean)
            counter = 0
            If Clock IsNot Nothing Then
                If AnimateClock = True Then
                    Animate = True
                Else
                    Animate = False
                End If
                Clock.Refresh()
            End If
        End Sub


#Region "FONT"
        Protected Overridable Function findRecomendedFontSizeAndStyle(ByRef ClockFontStyle As FontStyle) As Single
            ' recommended font size for (50,50) inner square and 15 circle weight is 12, hence the rest is derivative 
            ' 1. the minimum readable font size is 6 regular with "Bernard MT" font family
            ' 2. the minimum size of clock for the minimum font is inner square of 20 (clock is 38 if INNER_RECT_RATIO=0.5) and OuterCircle \ InnerCircle Weight is 4 
            ' 3. less than the minimum conditions above there will be no text inside the clock
            Dim innerSquareSide As Integer = OuterRectangleWidth * INNER_RECT_RATIO

            If innerSquareSide < 20 And OuterCircleWeight > 4 And InnerCircleWeight > 4 Then Return 1 ' no clock string under those conditions

            Dim result As Single = 6
            Dim PercentRatio As Single = innerSquareSide / 50

            If innerSquareSide > 48 And innerSquareSide < 52 Then
                Return 12
            Else
                If 12 * PercentRatio < 6 Then Return 1
                If 12 * PercentRatio < 7 Then ClockFontStyle = FontStyle.Regular
                Return 12 * PercentRatio
            End If

        End Function

        Public Overridable Function LoadFont() As Font
            Dim ClockFontStyle As FontStyle = FontStyle.Bold
            ' find font em size and style
            Dim fontsize As Single = findRecomendedFontSizeAndStyle(ClockFontStyle)
            If fontsize = 1 Then ' colock is to small: no font
                Return Nothing
            Else
                Return New Font("Bernard MT", fontsize, ClockFontStyle)
            End If
        End Function

        ''' <summary>
        ''' set new font for the clock string from outside GoalsClock class
        ''' </summary>
        ''' <param name="ClocksFontFaimily"></param>
        ''' <param name="ClockEmSize"></param>
        ''' <param name="ClockFontStyle"></param>
        ''' <remarks></remarks>
        Public Function LoadFont(ByVal ClocksFontFaimily As String, ByVal ClockEmSize As Single, ByVal ClockFontStyle As FontStyle) As Font
            Dim NewClockFont = New Font(ClocksFontFaimily, ClockEmSize, ClockFontStyle)
            ' set new font
            Me.ClockFont = NewClockFont
            Return NewClockFont
        End Function
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Animate = False
                    Me.Clock.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region



    End Class

    Public Interface IClock
        Enum AnimationLength
            SuperFast = 1
            Fast = 4
            ModerateFast = 8
            Moderate = 15
            ModerateSlow = 28
            Slow = 50
            SuperSlow = 80
        End Enum
    End Interface


    Public Class WaitClock
        Inherits GoalsClock

        Private _WaitText As String
        ''' <summary>
        ''' Gets or sets the text at the middle of the clock
        ''' </summary>
        ''' <returns></returns>
        Public Property WaitText As String
            Get
                Return _WaitText
            End Get
            Set(value As String)
                _WaitText = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Private Waitcolors() As Color = {GUI.Red, GUI.Green, GUI.Blue}

        Sub New(ByVal clockWidth As Int16, ByVal clockHeight As Int16, ByVal waitText As String)
            MyBase.New(clockWidth, clockHeight, 0)
            Me.WaitText = waitText
            ' recomended time for animation 
            TimerInterval = 28
        End Sub

        Public Overrides Sub Create(ByVal AnimateClock As Boolean)
            If AnimateClock = True Then
                Me.Animate = True
            Else
                Me.Animate = False
            End If
            ' panel
            Clock.BackColor = ClockBackGroundColor
            Clock.Size = New Size(ClockWidth, ClockHeight)
            ' subscribe to the panel's paint event
            AddHandler Clock.Paint, AddressOf clockPanel_Paint
        End Sub

        ''' <summary>
        ''' Sets a series of colors that will change after each 360 deg rotation
        ''' </summary>
        ''' <param name="arrayOfColors">array of Color</param>
        Public Sub SetArrayColors(ByVal arrayOfColors() As Color)
            If arrayOfColors.Count > 0 Then
                Waitcolors = Nothing
                Waitcolors = arrayOfColors
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End If
        End Sub

        Private counter As Int16 = 0
        Private colorCounter As Byte = 0
        Protected Overrides Sub clockPanel_Paint(sender As Object, e As PaintEventArgs)
            Dim Gclock As Graphics = e.Graphics 'Local variable only, as the Graphics object might change.
            Gclock.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            Gclock.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit

            ' create outer rectangle
            OuterRect = New Rectangle(xPos, yPos, ClockWidth - OuterCircleWeight - 2, ClockHeight - OuterCircleWeight - 2)
            ' create inner rectangle
            Dim w, h, x, y As Integer
            getInnerRectSizeAndLocation(w, h, x, y)
            InnerRect = New Rectangle(x, y, w, h)
            ' create outer circle
            OuterCirclePen = New Pen(OuterCircleColor, OuterCircleWeight)

            Gclock.DrawArc(OuterCirclePen, OuterRect, 1.0F, 360.0F)
            ' create inner circle


            Dim sweepAngle As Short = getSweepAngleFromGoalsPercent()

            If ClockFont IsNot Nothing Then ' if nothing LoadFont() determined that clock is to small for text inside
                ' draw goals string inside inner rect
                InnerStringBrush = New SolidBrush(FontColor)
                ' Create a StringFormat object with the each line of text, and the block
                ' of text centered on the page.
                Dim stringFormat As New StringFormat()
                stringFormat.Alignment = StringAlignment.Center
                stringFormat.LineAlignment = StringAlignment.Center
                Gclock.DrawString(WaitText, ClockFont, InnerStringBrush, InnerRect, stringFormat)
            End If

            If Animate = True Then
                If IsNothing(_TimerAnimation) Then
                    InnerCirclePen = New Pen(InnerCircleColor, InnerCircleWeight)
                    InnerCirclePen.Color = Waitcolors(0)
                    TimerAnimatiom.Interval = TimerInterval
                    AddHandler TimerAnimatiom.Tick, AddressOf TimerAnimatiom_Tick
                    TimerAnimatiom.Start()
                End If
            Else
                InnerCirclePen = New Pen(InnerCircleColor, InnerCircleWeight)
            End If

            Gclock.DrawArc(InnerCirclePen, OuterRect, -90.5F, counter)


        End Sub

        Protected Overrides Sub TimerAnimatiom_Tick(sender As Object, e As EventArgs)


            If Animate = False Then
                counter = 0
                TimerAnimatiom.Stop()
                TimerAnimatiom.Dispose()
                TimerAnimatiom = Nothing
                Exit Sub
            End If


            If counter >= 360 Then
                counter = 0
                If colorCounter = Waitcolors.Count Then
                    colorCounter = 0
                    InnerCirclePen.Color = Waitcolors(colorCounter)
                    colorCounter += 1
                Else
                    InnerCirclePen.Color = Waitcolors(colorCounter)
                    colorCounter += 1
                End If
            Else
                counter += 4
            End If

            Clock.Invalidate(True)
        End Sub
    End Class



    Public Class CarClock
        Inherits GoalsClock
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="clockWidth">width of clock</param>
        ''' <param name="clockHeight">height of clock</param>
        ''' <param name="GoalsPercent">performance in percents</param>
        ''' <param name="AddBaseBar">will add a bar in the buttom of the clock note: it will increase the height in 30px</param>
        ''' <remarks>if bar base is set to true it will increase the height in 30px</remarks>
        Sub New(ByVal clockWidth As Int16, ByVal clockHeight As Int16, ByVal GoalsPercent As Single, Optional AddBaseBar As Boolean = False)
            MyBase.New(clockWidth, clockHeight, GoalsPercent)
            ' will be used for animation if set to true
            OriginalPerformanceValue = GoalsPercent
            ' recomended interval for animtiom 
            TimerInterval = 8

            If AddBaseBar = True Then
                Me.BaseBar = True
                ' initilize clocks panel to hold the clock
                ClockPanel = New Panel()
            End If


        End Sub


        Protected _LowPerformance As Single = 0.4
        ''' <summary>
        ''' Gets or sets the indication for low performance, this property is binded to LowPerFormanceColor property and its value will set the graphical area of the low performance clock with LowPerFormanceColor color
        ''' </summary>
        ''' <returns></returns>
        Public Property LowPerformance As Single
            Get
                Return _LowPerformance
            End Get
            Set(value As Single)
                _LowPerformance = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Protected _MediumPerformance As Single = 0.7
        ''' <summary>
        ''' Gets or sets the indication for Medium performance, this property is binded to MediumPerFormanceColor property and its value will set the graphical area of the Medium performance clock with MediumPerFormanceColor color
        ''' </summary>
        ''' <returns></returns>
        Public Property MediumPerformance As Single
            Get
                Return _MediumPerformance
            End Get
            Set(value As Single)
                _MediumPerformance = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Protected _HighPerformance As Single = 1.0
        ''' <summary>
        ''' Gets or sets the indication for High performance, this property is binded to HighPerFormanceColor property and its value will set the graphical area of the High performance clock with HighPerFormanceColor color
        ''' </summary>
        ''' <returns></returns>
        Public Property HighPerformance As Single
            Get
                Return _HighPerformance
            End Get
            Set(value As Single)
                _HighPerformance = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property


        Protected _HighPerFormanceColor As Color = Color.FromArgb(129, 253, 129)
        ''' <summary>
        ''' Gets or sets the graphical High performance area in the clock with the color that was selected
        ''' </summary>
        ''' <returns></returns>
        Public Property HighPerFormanceColor As Color
            Get
                Return _HighPerFormanceColor
            End Get
            Set(value As Color)
                _HighPerFormanceColor = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Protected _MediumPerFormanceColor As Color = Color.FromArgb(253, 253, 150)
        ''' <summary>
        ''' Gets or sets the graphical medium performance area in the clock with the color that was selected
        ''' </summary>
        ''' <returns></returns>
        Public Property MediumPerFormanceColor As Color
            Get
                Return _MediumPerFormanceColor
            End Get
            Set(value As Color)
                _MediumPerFormanceColor = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Protected _LowPerFormanceColor As Color = Color.FromArgb(255, 105, 97)

        ''' <summary>
        ''' Gets or sets the graphical low performance area in the clock with the color that was selected
        ''' </summary>
        ''' <returns></returns>
        Public Property LowPerFormanceColor As Color
            Get
                Return _LowPerFormanceColor
            End Get
            Set(value As Color)
                _LowPerFormanceColor = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property

        Protected NeedlePanel As PictureBox

        Protected _NeedleBaseColor As Color = Color.FromArgb(40, 40, 40)
        ''' <summary>
        ''' Gets or sets the background color of the needle (clock hand) base
        ''' </summary>
        ''' <returns></returns>
        Public Property NeedleBaseColor As Color
            Get
                Return _NeedleBaseColor
            End Get
            Set(value As Color)
                _NeedleBaseColor = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Protected _NeedleBaseWeight As Byte = OuterCircleWeight / 2
        ''' <summary>
        ''' Gets or sets the pixel weight (thickness) of the needle (clock hand) base
        ''' </summary>
        ''' <returns></returns>
        Public Property NeedleBaseWeight As Byte
            Get
                Return _NeedleBaseWeight
            End Get
            Set(value As Byte)
                _NeedleBaseWeight = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property

        Protected _NeedleOuterColor As Color = Color.FromArgb(50, 50, 50)
        ''' <summary>
        ''' Gets or sets the background color of the outer needle (clock hand) area
        ''' </summary>
        ''' <returns></returns>
        Public Property NeedleOuterColor As Color
            Get
                Return _NeedleOuterColor
            End Get
            Set(value As Color)
                _NeedleOuterColor = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Protected _NeedleOuterWeight As Byte = OuterCircleWeight / 2
        ''' <summary>
        '''  Gets or sets the pixel weight (thickness) of the outer needle (clock hand) area
        ''' </summary>
        ''' <returns></returns>
        Public Property NeedleOuterWeight As Byte
            Get
                Return _NeedleOuterWeight
            End Get
            Set(value As Byte)
                _NeedleOuterWeight = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property

        Protected _NeedleInnerColor As Color = Color.FromArgb(129, 253, 129)
        ''' <summary>
        '''  Gets or sets the background color of the inner needle (clock hand) area
        ''' </summary>
        ''' <returns></returns>
        Public Property NeedleInnerColor As Color
            Get
                Return _NeedleInnerColor
            End Get
            Set(value As Color)
                _NeedleInnerColor = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Protected _NeedleInnerWeight As Byte = NeedleOuterWeight / 3
        ''' <summary>
        ''' Gets or sets the pixel weight (thickness) of the inner needle (clock hand) area
        ''' </summary>
        ''' <returns></returns>
        Public Property NeedleInnerWeight As Byte
            Get
                Return _NeedleInnerWeight
            End Get
            Set(value As Byte)
                _NeedleInnerWeight = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property
        Protected _ShowInnerCutForPerformance As Boolean = True
        ''' <summary>
        ''' Gets or sets a value indicating whether visual inner cut (slash) that represents performance will be drawn in the middle of the clock border
        ''' </summary>
        ''' <returns></returns>
        Public Property ShowInnerCutForPerformance As Boolean
            Get
                Return _ShowInnerCutForPerformance
            End Get
            Set(value As Boolean)
                _ShowInnerCutForPerformance = value
                If Clock IsNot Nothing Then
                    RefreshClock(Animate)
                End If
            End Set
        End Property

        Protected _InnerCutPreformanceColor As Color = ClockBackGroundColor
        ''' <summary>
        ''' Gets or sets the color of the visual inner cut (slash) that represents performance in the middle of the clock border
        ''' </summary>
        ''' <returns></returns>
        Public Property InnerCutPreformanceColor As Color
            Get
                Return _InnerCutPreformanceColor
            End Get
            Set(value As Color)
                _InnerCutPreformanceColor = value
            End Set
        End Property
        Protected _InnerCutPerformanceWeight As Byte = 2
        ''' <summary>
        ''' Gets or sets the weight (thickness) of the inner cut (slash) that represents performance in the middle of the clock border
        ''' </summary>
        ''' <returns></returns>
        Public Property InnerCutPerformanceWeight As Byte
            Get
                Return _InnerCutPerformanceWeight
            End Get
            Set(value As Byte)
                _InnerCutPerformanceWeight = value
            End Set
        End Property


        ' animation vars
        Protected animationTimer As Timer
        Protected _OriginalPerformanceValue As Single
        Protected Property OriginalPerformanceValue As Single
            Get
                Return _OriginalPerformanceValue
            End Get
            Set(value As Single)
                _OriginalPerformanceValue = value
            End Set
        End Property



        ''' <summary>
        ''' use to contain the Clock object if text bar is required. Important: 
        ''' When BaseBar = True the Clock object wil be hosted inside clock panel in order to present a bar of performance text underneath the clock Important:
        ''' 1. If BaseBar = True use this control (and not Clock property) to get the clock with the text bar.
        ''' 2. If BaseBar = False never use this property, the result will be NullReferenceException because it will not be initilized (Please read that issue: https://github.com/Jonathan435/Business-Clocks-Charts/issues/6).
        ''' </summary>
        Public ClockPanel As Panel
        Protected StopAnimating As Boolean = False

        Public Overrides Sub Create(AnimateClock As Boolean)
            MyBase.Create(AnimateClock)
            ' a panel to hold the needle
            NeedlePanel = New PictureBox
            NeedlePanel.BackColor = Color.Transparent
            NeedlePanel.Size = New Size(RectNeedle_Width, RectNeedle_Width)
            Clock.Controls.Add(NeedlePanel)
            ' barbase will add everything to a panel
            If BaseBar = True Then
                LoadBarValues()
                Clock.Location = New Point(0, 0)
                ClockPanel.Controls.Add(Clock)
            End If
        End Sub

        Private Shared testcounter As Integer

        Protected Overridable Sub InnerHandClockPanel_Paint(sender As Object, e As PaintEventArgs)

            Dim Gclock As Graphics = e.Graphics
            Gclock.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            Gclock.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit
            ' create a rectangle for circle
            Dim needlePen As New Pen(NeedleOuterColor, NeedleOuterWeight) ' for test
            RectHandClock = New Rectangle(0, 0, RectNeedle_Width, RectNeedle_Width)
            ' draw the clock hand
            Gclock.DrawPolygon(needlePen, New Point() {NeedleFirstPoint, getHandClockSecondPoint()})
            ' draw inndr needle
            Dim innerNeedlePen As New Pen(NeedleInnerColor, NeedleInnerWeight)
            Gclock.DrawPolygon(innerNeedlePen, New Point() {New Point(NeedleFirstPoint.X, NeedleFirstPoint.Y), New Point(NeedleFirstPoint.X, NeedleFirstPoint.Y), getHandClockSecondPoint()})
            ' draw the base of the needle
            Dim needleBasePen As New Pen(NeedleBaseColor, NeedleBaseWeight)
            ' rectangle and arc for base
            Dim baseRect As New Rectangle(NeedleFirstPoint.X - 3, NeedleFirstPoint.Y - 3, 6, 6)
            Gclock.DrawArc(needleBasePen, baseRect, 0.0F, 360.0F)


            testcounter += 1


        End Sub


        Protected Overrides Sub clockPanel_Paint(sender As Object, e As PaintEventArgs)

            'Try
            Dim Gclock As Graphics = e.Graphics 'Local variable only, as the Graphics object might change.
            Gclock.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            Gclock.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit


            ' create outer rectangle
            OuterRect = New Rectangle(xPos, yPos, ClockWidth - OuterCircleWeight - 2, (ClockHeight - OuterCircleWeight - 2) * 2)
            '   Gclock.DrawRectangle(New Pen(Color.Yellow, 2), OuterRect)
            ' create inner rectangle
            Dim w, h, x, y As Integer
            getInnerRectSizeAndLocation(w, h, x, y)
            InnerRect = New Rectangle(x, y, w, h)
            ' create outer circle
            OuterCirclePen = New Pen(OuterCircleColor, OuterCircleWeight)
            ' draw part of arc by performance parameters
            Dim LengthOfArc As Single = 0
            Gclock.DrawArc(New Pen(LowPerFormanceColor, OuterCircleWeight), OuterRect, getColorDeg(Performance.Lowperformance, LengthOfArc), LengthOfArc)
            Gclock.DrawArc(New Pen(MediumPerFormanceColor, OuterCircleWeight), OuterRect, getColorDeg(Performance.MediumPerformance, LengthOfArc), LengthOfArc)
            Gclock.DrawArc(New Pen(HighPerFormanceColor, OuterCircleWeight), OuterRect, getColorDeg(Performance.HighPerformance, LengthOfArc), LengthOfArc)

            ' handle the panel that holds the needle
            NeedlePanel.Location = New Point(RectNeedle_X, RectNeedle_Y)

            If Animate = False And PercentOfGoals > 0 Then
                ' subscribe to the panel's paint event
                AddHandler NeedlePanel.Paint, AddressOf InnerHandClockPanel_Paint
                ' make cut inside arc until the goals performance

                If ShowInnerCutForPerformance = True Then
                    Gclock.DrawArc(New Pen(InnerCutPreformanceColor, InnerCutPerformanceWeight) _
                                , New Rectangle(OuterRect.X, OuterRect.Y, OuterRect.Width, OuterRect.Height) _
                                , -180.0F, MyBase.getSweepAngleFromGoalsPercent() / 2)
                End If

                ' if there is a bar
                If BaseBar = True Then
                    lblPerformance.Text = FormatPercent(GoalsPercentDecimalValue, 0) & " "
                End If

            Else

                If ShowInnerCutForPerformance = True Then
                    Gclock.DrawArc(New Pen(InnerCutPreformanceColor, 2) _
                                , New Rectangle(OuterRect.X, OuterRect.Y, OuterRect.Width, OuterRect.Height) _
                                , -180.0F, MyBase.getSweepAngleFromGoalsPercent() / 2)
                End If

                If StopAnimating = True Then
                    Exit Sub
                End If

                ' will be increment to the original value via _OriginalPerformanceValue
                If IsNothing(animationTimer) Then
                    animationTimer = New Timer
                    PercentOfGoals = 0.0
                    AddHandler NeedlePanel.Paint, AddressOf InnerHandClockPanel_Paint
                    animationTimer.Interval = TimerInterval
                    AddHandler animationTimer.Tick, AddressOf animationTimer_Tick
                    animationTimer.Start()

                End If
            End If

            Debug.WriteLine("Clock paint event was called!") ' TODO: DELETE

            'Catch ex As Exception
            '    Debug.WriteLine("ERROR: " & ex.Message.ToString)
            'End Try

        End Sub

        Private Sub animationTimer_Tick(sender As Object, e As EventArgs)
            If GoalsPercentDecimalValue >= OriginalPerformanceValue Or GoalsPercentDecimalValue > 1 Then
                PercentOfGoals = OriginalPerformanceValue
                ' if there is a bar make sure the number is accurate
                If BaseBar = True Then
                    lblPerformance.Text = FormatPercent(GoalsPercentDecimalValue, 0) & " "
                End If
                Clock.Invalidate(True)
                animationTimer.Stop()
                animationTimer.Dispose()
                animationTimer = Nothing
                Animate = False
                StopAnimating = True
                Exit Sub
            End If

            ' if there is a bar animate the string
            If BaseBar = True Then
                lblPerformance.Text = FormatPercent(GoalsPercentDecimalValue, 0) & " "
            End If

            PercentOfGoals = GoalsPercentDecimalValue + Single.Parse(0.05)
            Clock.Invalidate(True)


        End Sub

        Protected Function getHandClockSecondPoint() As Point

            ' convert the performance to another Level of measurement: performance of 0 = -90 perfoarmnce of 1.0 = 90
            Dim c As Integer = 0

            If GoalsPercentDecimalValue <= 0 Then
                c = -90
            ElseIf GoalsPercentDecimalValue >= 1 Then
                c = 90
            ElseIf GoalsPercentDecimalValue < 0.5 Then
                c = -90 + (GoalsPercentDecimalValue * 180)
            Else
                c = (GoalsPercentDecimalValue * 180) - 90
            End If

            Dim x As Integer = NeedlePanel.Width / 2 + ((NeedlePanel.Width / 2) * Math.Sin(Math.PI * c / 180))
            Dim y As Integer = NeedlePanel.Width / 2 - ((NeedlePanel.Width / 2) * Math.Cos(Math.PI * c / 180))

            Return New Point(x, y)
        End Function

        Protected RectHandClock As Rectangle

        Protected ReadOnly Property RectNeedle_Width As Int16
            Get
                Return ClockWidth / 1.3
            End Get
        End Property

        Protected ReadOnly Property RectNeedle_X
            Get
                '   Return ClockWidth / 2 - RectForClockHand_Width / 2
                Return (OuterRect.Width / 2 + OuterRect.X) - RectNeedle_Width / 2
            End Get
        End Property

        Protected ReadOnly Property RectNeedle_Y
            Get
                '  Return ClockHeight / 2 - RectForClockHand_Width / 2
                Return (OuterRect.Height / 2 + OuterRect.Y) - RectNeedle_Width / 2
            End Get
        End Property
        ' the middle of the circle
        Protected ReadOnly Property NeedleFirstPoint() As Point
            Get
                Return New Point(NeedlePanel.Width / 2, NeedlePanel.Height / 2)
            End Get
        End Property

        ' the length from the start of the hand to the end (the circle / squar border)
        ' variable to calculate the angle
        Protected ReadOnly Property NeedleCircle_Radius As Int16
            Get
                Return NeedlePanel.Width / 2
            End Get
        End Property

        Private Function getColorDeg(ByVal p As Performance, ByRef LengthOfArc As Single) As Single

            Dim strPoint As Short = -180
            Select Case p

                Case Performance.Lowperformance
                    LengthOfArc = Math.Abs(strPoint) * LowPerformance
                    Return strPoint - 1
                Case Performance.MediumPerformance
                    LengthOfArc = Math.Abs(strPoint) * Math.Abs((MediumPerformance - LowPerformance))
                    Return strPoint + Math.Abs(strPoint) * LowPerformance
                Case Performance.HighPerformance
                    LengthOfArc = Math.Abs(strPoint) * Math.Abs((HighPerformance - MediumPerformance))
                    Return (strPoint + Math.Abs(strPoint) * MediumPerformance) + 1
                Case Else
                    Return Nothing
            End Select

        End Function

        Public Overrides Sub RefreshClock(AnimateClock As Boolean)
            ' possible (high chance) for updating the performance from outside using PercentOfGoals
            StopAnimating = False
            OriginalPerformanceValue = GoalsPercentDecimalValue
            MyBase.RefreshClock(AnimateClock)
        End Sub
        Enum Performance
            Lowperformance = 0
            MediumPerformance = 1
            HighPerformance = 2
        End Enum

        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)
            If Not (IsNothing(ClockPanel)) Then
                ClockPanel.Dispose()
            End If
        End Sub

#Region "BASE BAR"
        Protected BaseBar As Boolean = False
        Protected lblPerformance As Label
        ' inner properties - dependent on clocks outer properties
        Private _BarBackColor As Color = ClockBackGroundColor
        ''' <summary>
        ''' Gets or sets the performance bar background color
        ''' </summary>
        ''' <returns></returns>
        Public Property BarBackColor As Color
            Get
                Return _BarBackColor
            End Get
            Set(value As Color)
                _BarBackColor = value
                _BarValueDigitsBackColor = value
                If ClockPanel IsNot Nothing AndAlso lblPerformance IsNot Nothing AndAlso lblMarkMax IsNot Nothing AndAlso lblMarkMin IsNot Nothing Then
                    lblPerformance.BackColor = _BarBackColor
                    lblMarkMin.BackColor = _BarBackColor
                    lblMarkMax.BackColor = _BarBackColor
                End If
            End Set
        End Property


        Private lblMarkMin, lblMarkMax As New Label
        Private _BarValueDigitsForeColor As Color = NeedleInnerColor
        ''' <summary>
        ''' Gets or sets the font color of the bar's max(100%) and min(0%) lables
        ''' </summary>
        ''' <returns></returns>
        Public Property BarValueDigitsForeColor As Color
            Get
                Return _BarValueDigitsForeColor
            End Get
            Set(value As Color)
                _BarValueDigitsForeColor = value
                If ClockPanel IsNot Nothing AndAlso lblPerformance IsNot Nothing AndAlso lblMarkMax IsNot Nothing AndAlso lblMarkMin IsNot Nothing Then
                    lblMarkMax.ForeColor = _BarValueDigitsForeColor
                    lblMarkMin.ForeColor = _BarValueDigitsForeColor
                End If
            End Set
        End Property

        Private _BarValueDigitsBackColor = ClockBackGroundColor
        ''' <summary>
        ''' Gets or sets the background color of the bar's max(100%) and min(0%) lables
        ''' </summary>
        ''' <returns></returns>
        Public Property BarValueDigitsBackColor As Color
            Get
                Return _BarValueDigitsBackColor
            End Get
            Set(value As Color)
                _BarValueDigitsBackColor = value
                If ClockPanel IsNot Nothing AndAlso lblPerformance IsNot Nothing AndAlso lblMarkMax IsNot Nothing AndAlso lblMarkMin IsNot Nothing Then
                    lblMarkMax.BackColor = _BarValueDigitsBackColor
                    lblMarkMin.BackColor = _BarValueDigitsBackColor
                End If
            End Set
        End Property

        Protected _BarFont As Font = New Font("Bernard MT", 16, FontStyle.Bold)
        ''' <summary>
        ''' Gets or sets the font of the text that describes the PercentOfGoals percentage
        ''' </summary>
        ''' <returns></returns>
        Public Property BarFont As Font
            Get
                Return _BarFont
            End Get
            Set(value As Font)
                _BarFont = value
                If ClockPanel IsNot Nothing Then
                    Me.lblPerformance.Font = _BarFont
                End If
            End Set
        End Property

        Private _BarHeight As Int16 = 30
        Protected Property BarHeight As Int16
            Get
                Return _BarHeight
            End Get
            Set(value As Int16)
                _BarHeight = value
                If ClockPanel IsNot Nothing AndAlso lblPerformance IsNot Nothing AndAlso lblMarkMax IsNot Nothing AndAlso lblMarkMin IsNot Nothing Then
                    lblMarkMax.Height = _BarHeight
                    lblMarkMin.Height = _BarHeight
                    lblPerformance.Height = _BarHeight
                End If
            End Set
        End Property

        Protected _BarForeColor As Color = Color.White
        ''' <summary>
        ''' Gets or sets the performance bar font color
        ''' </summary>
        ''' <returns></returns>
        Public Property BarForeColor As Color
            Get
                Return _BarForeColor
            End Get
            Set(value As Color)
                _BarForeColor = value
                If ClockPanel IsNot Nothing AndAlso lblPerformance IsNot Nothing AndAlso lblMarkMax IsNot Nothing AndAlso lblMarkMin IsNot Nothing Then
                    Me.lblPerformance.ForeColor = _BarForeColor
                End If
            End Set
        End Property

        Protected _BarValueDigitsFont As Font = New Font("ARIAL", 8, FontStyle.Regular)
        ''' <summary>
        '''  Gets or sets the font of the bar's max(100%) and min(0%) lables
        ''' </summary>
        ''' <returns></returns>
        Public Property BarValueDigitsFont As Font
            Get
                Return _BarValueDigitsFont
            End Get
            Set(value As Font)
                _BarValueDigitsFont = value
                If ClockPanel IsNot Nothing AndAlso lblPerformance IsNot Nothing AndAlso lblMarkMax IsNot Nothing AndAlso lblMarkMin IsNot Nothing Then
                    lblMarkMax.Font = _BarValueDigitsFont
                    lblMarkMin.Font = _BarValueDigitsFont
                End If
            End Set
        End Property


        Sub LoadBarValues()

            ' panel defenitions
            ClockPanel.Size = New Size(ClockWidth, ClockHeight + BarHeight)
            ClockPanel.BackColor = BarBackColor

            ' ADD CONTROLS
            ' digit mark lables

            MarkLablesSharedProperties(lblMarkMin, "  0%")
            MarkLablesSharedProperties(lblMarkMax, "100%")

            lblMarkMin.Location = New Point(0, ClockPanel.Height - BarHeight)
            lblMarkMin.TextAlign = ContentAlignment.TopRight

            lblMarkMax.Location = New Point(ClockWidth - lblMarkMax.Width, ClockPanel.Height - BarHeight)
            lblMarkMax.TextAlign = ContentAlignment.TopLeft

            ClockPanel.Controls.Add(lblMarkMin)
            ClockPanel.Controls.Add(lblMarkMax)

            ' create the performance string label
            lblPerformance = New Label
            lblPerformance.Size = New Size(ClockPanel.Width - (lblMarkMax.Width * 2), BarHeight)
            lblPerformance.Location = New Point(lblMarkMin.Width, ClockPanel.Height - BarHeight)
            lblPerformance.BackColor = BarBackColor
            lblPerformance.ForeColor = BarForeColor
            lblPerformance.Font = BarFont
            lblPerformance.TextAlign = ContentAlignment.BottomCenter
            lblPerformance.Text = "0% "

            ClockPanel.Controls.Add(lblPerformance)

        End Sub

        Private Sub MarkLablesSharedProperties(ByRef L As Label, ByVal txt As String)

            L.Size = New Size(36, BarHeight)
            L.Text = txt
            L.BackColor = BarValueDigitsBackColor
            L.ForeColor = BarValueDigitsForeColor
            L.Font = BarValueDigitsFont

        End Sub

#End Region

    End Class

    Public Module GUI
        Public Green As Color = Color.FromArgb(129, 253, 129)
        Public Yellow As Color = Color.FromArgb(253, 253, 150)
        Public Red As Color = Color.FromArgb(255, 105, 97)
        Public Blue As Color = Color.FromArgb(101, 255, 229)
    End Module

End Namespace
