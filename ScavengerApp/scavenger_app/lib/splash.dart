import 'package:flutter/material.dart';
import 'package:scavenger_app/home.dart';
import 'package:scavenger_app/scavenger_api.dart';

class SplashScreen extends StatefulWidget {
  const SplashScreen({super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen> {
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    try {
      final response = await ScavengerAPI().start();

      // Check response from API here
      // For demonstration, I'm assuming a successful response
      // Replace this with actual API call and response handling
      _navigateToMainScreen(response["scavengerId"]);
    } catch (e) {
      print('Error loading data: $e');
      // Handle error
    }
  }

  void _navigateToMainScreen(String scavengerId) {
    setState(() {
      _isLoading = false;
    });
    Navigator.of(context).pushReplacement(
      MaterialPageRoute(
          builder: (context) => Home(
                scavengerId: scavengerId,
              )),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: _isLoading
            ? const CircularProgressIndicator()
            : const Text('Data loaded. Navigating to main screen...'),
      ),
    );
  }
}
