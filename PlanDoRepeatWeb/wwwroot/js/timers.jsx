class Timer extends React.Component {
    render() {
        return (<div className="timer">
            <div className="timer-body">{this.props.name} : {this.props.desc}</div>
            <div className="timer-button timer-button-repeat"></div>
            <div className="timer-button timer-button-play"></div>
            <div className="timer-button timer-button-stop"></div>
            <div className="timer-button timer-button-delete"></div>
        </div>);
    }
}

class TimerList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            timers: []
        };
    }
    
    renderTimer(timerProps){
        return (<Timer key={timerProps.Id} name={timerProps.Name} desc={timerProps.Description} value={timerProps}/>);
    }

    componentDidMount() {
        fetch("/TimerApi/GetAllTimers")
            .then(res => res.json())
            .then(
                result => {
                    this.setState({
                        error: null,
                        isLoaded: true,
                        timers: result
                    });
                },
                (error) => {
                    this.setState({
                        error: error.message,
                        isLoaded: true,
                        timers: null
                    });
                })
    }

    render() {
        let status = "";
        if (!this.state.isLoaded) {
            status = "Загрузка...";
        } else if (this.state.error) {
            status = this.state.error;
        } else {
            status = this.state.timers.map(item => this.renderTimer(item));
        }
        return (<div className="timers-list">{status}</div>);
    }
}

ReactDOM.render(
    <TimerList/>,
    document.getElementById("content")
);