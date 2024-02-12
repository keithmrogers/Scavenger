import 'dart:async';
import 'dart:convert';
import 'dart:math';
import 'package:flutter/material.dart';
import 'package:geolocator/geolocator.dart';
import 'package:http/http.dart' as http;
import 'package:sensors_plus/sensors_plus.dart';

class Home extends StatefulWidget {
  final String scavengerId;

  const Home({super.key, required this.scavengerId});

  @override
  State<Home> createState() => _HomeState();
}

class _HomeState extends State<Home> {
  StreamSubscription<Position>? _positionStreamSubscription;
  StreamSubscription<AccelerometerEvent>? _compassStreamSubscription;
  Position? _currentPosition;
  double? _currentHeading;

  @override
  void initState() {
    super.initState();
    _initPlatformState();
  }

  @override
  void dispose() {
    _positionStreamSubscription?.cancel();
    _compassStreamSubscription?.cancel();
    super.dispose();
  }

  Future<void> _initPlatformState() async {
    // Ask for location permissions if not granted
    final LocationPermission geolocationStatus =
        await Geolocator.checkPermission();
    if (geolocationStatus == LocationPermission.denied) {
      await Geolocator.requestPermission();
    }

    // Start listening to position updates
    _positionStreamSubscription = Geolocator.getPositionStream(
            locationSettings: const LocationSettings(
                accuracy: LocationAccuracy.best, distanceFilter: 1))
        .listen((Position position) {
      setState(() {
        _currentPosition = position;
      });
    });

    // Start listening to compass updates
    _compassStreamSubscription =
        accelerometerEventStream.call().listen((AccelerometerEvent event) {
      double heading = _calculateCompassHeading(event);
      setState(() {
        _currentHeading = heading;
      });
    });
  }

  double _calculateCompassHeading(AccelerometerEvent event) {
    double x = event.x;
    double y = event.y;
    double radians = -atan2(y, x);
    double heading = radians * (180 / pi);
    return heading >= 0 ? heading : (360 + heading);
  }

  Future<void> _sendDataToApi() async {
    if (_currentPosition != null && _currentHeading != null) {
      try {
        final url = Uri.parse('YOUR_API_ENDPOINT_HERE');
        final response = await http.post(
          url,
          body: jsonEncode({
            'latitude': _currentPosition!.latitude,
            'longitude': _currentPosition!.longitude,
            'heading': _currentHeading!,
          }),
          headers: <String, String>{
            'Content-Type': 'application/json; charset=UTF-8',
          },
        );
        if (response.statusCode == 200) {
          print('Data sent successfully');
        } else {
          print('Failed to send data. Status code: ${response.statusCode}');
        }
      } catch (e) {
        print('Error sending data: $e');
      }
    } else {
      print('No data available to send');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('GPS & Compass Sender'),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            if (_currentPosition != null)
              Text(
                'Latitude: ${_currentPosition!.latitude}, Longitude: ${_currentPosition!.longitude}',
              ),
            if (_currentHeading != null)
              Text(
                'Heading: ${_currentHeading?.toStringAsFixed(2)}°',
              ),
            const SizedBox(height: 20),
            ElevatedButton(
              onPressed: _sendDataToApi,
              child: const Text('Send Data to API'),
            ),
          ],
        ),
      ),
    );
  }
}
