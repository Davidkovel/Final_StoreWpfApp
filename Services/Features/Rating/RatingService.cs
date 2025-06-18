using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core.Repository;

namespace Services.Features.Rating;

public class RatingService : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IRatingRepository _ratingRepository;
    private double _averageRating;
    private bool _hasUserRated;

    public RatingService(IRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
        Ratings = new ObservableCollection<int>();
    }

    public ObservableCollection<int>? Ratings { get; }

    public double AverageRating
    {
        get => _averageRating;
        private set
        {
            if (_averageRating != value)
            {
                _averageRating = value;
                OnPropertyChanged();
            }
        }
    }

    public bool HasUserRated
    {
        get => _hasUserRated;
        private set
        {
            if (_hasUserRated != value)
            {
                _hasUserRated = value;
                OnPropertyChanged();
            }
        }
    }

    public async Task LoadRatingsAsync(int productId)
    {
        Ratings.Clear();
        var ratings = await _ratingRepository.GetRatingsByProductIdAsync(productId);
        Console.WriteLine(ratings);
        foreach (var rating in ratings)
        {
            Console.WriteLine($"{rating.Rating} - {rating.UserId}");
            Ratings.Add(rating.Rating ?? 0);
        }

        CalculateAverageRating();
    }

    public async Task LoadUserRatingAsync(string userId, int productId)
    {
        var userRatings = await _ratingRepository.GetRatingsByUserIdAsync(userId, productId);
        HasUserRated = await _ratingRepository.CheckIfUserHasRatedAsync(userId, productId);
    }

    public async Task AddRatingAsync(int selectedRating, int productId, string userId)
    {
        await _ratingRepository.AddRatingAsync(selectedRating, productId, userId);
        Ratings.Add(selectedRating);
        CalculateAverageRating();
        HasUserRated = true;
    }

    public async Task<bool> CheckIfUserHasRatedAsync(string userId, int productId)
    {
        return await _ratingRepository.HasUserRatedAsync(userId, productId);
    }

    private void CalculateAverageRating()
    {
        AverageRating = Ratings.Any()
            ? Ratings.Average()
            : 0.0;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}